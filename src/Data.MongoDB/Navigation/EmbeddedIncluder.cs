using CodeArchitects.Platform.Data.MongoDB.Model;
using CodeArchitects.Platform.Data.Navigation;
using MongoDB.Bson.Serialization;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Navigation;

/// <summary>
/// Validates the navigations requested through an <see cref="IncludeAction{TEntity}"/>.
/// </summary>
/// <remarks>
/// A navigation embedded in the document is already loaded with it, so including it has no
/// effect, as EF Core does with owned types. Every other request is rejected instead of being
/// ignored: a reference to a document of another collection, which the provider does not
/// resolve, and a member that is not persisted, which would stay empty.
/// </remarks>
internal sealed class EmbeddedIncluder<TEntity> : IIncluder<TEntity>
  where TEntity : class
{
  private readonly IDataModel _model;

  public EmbeddedIncluder(IDataModel model)
  {
    _model = model;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    if (includeExpression is null)
      throw new ArgumentNullException(nameof(includeExpression));

    switch (includeExpression.Body)
    {
      case MemberExpression memberExpression:
        ValidatePath(memberExpression);
        break;

      case NewExpression newExpression when newExpression.Type.IsAnonymousType():
        foreach (Expression argument in newExpression.Arguments)
        {
          if (argument is not MemberExpression memberExpression)
            throw InvalidExpression(argument);

          ValidatePath(memberExpression);
        }
        break;

      default:
        throw InvalidExpression(includeExpression.Body);
    }

    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    return Include(includeExpression?.Body, thenInclude);
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    return Include(includeExpression?.Body, thenInclude);
  }

  public IStringIncluder<TEntity> Include(string navigation)
  {
    if (string.IsNullOrWhiteSpace(navigation))
      throw new ArgumentException("The include path cannot be empty.", nameof(navigation));

    Type owner = typeof(TEntity);
    foreach (string segment in navigation.Split('.'))
    {
      if (segment.Length == 0)
        throw new InvalidOperationException($"The include path '{navigation}' has an empty segment.");

      MemberInfo member = (MemberInfo?)owner.GetProperty(segment, BindingFlags.Public | BindingFlags.Instance)
        ?? owner.GetField(segment, BindingFlags.Public | BindingFlags.Instance)
        ?? throw new InvalidOperationException(
          $"Navigation '{segment}' of the include path '{navigation}' was not found on '{owner.Name}'.");

      owner = ValidateSegment(owner, member);
    }

    return this;
  }

  private IExpressionIncluder<TEntity> Include<T>(Expression? body, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    if (body is null)
      throw new ArgumentNullException("includeExpression");
    if (thenInclude is null)
      throw new ArgumentNullException(nameof(thenInclude));

    if (body is not MemberExpression memberExpression)
      throw InvalidExpression(body);

    ValidatePath(memberExpression);

    // The sub-navigations are validated from the navigation's own type.
    thenInclude(new EmbeddedIncluder<T>(_model));

    return this;
  }

  /// <summary>
  /// Validates every segment of <c>x => x.A.B</c>, from the root down.
  /// </summary>
  private void ValidatePath(MemberExpression expression)
  {
    Stack<MemberInfo> members = new();
    Expression? current = expression;

    while (current is MemberExpression memberExpression)
    {
      members.Push(memberExpression.Member);
      current = memberExpression.Expression;
    }

    if (current is not ParameterExpression)
      throw InvalidExpression(expression);

    Type owner = typeof(TEntity);
    foreach (MemberInfo member in members)
    {
      owner = ValidateSegment(owner, member);
    }
  }

  /// <summary>
  /// Validates one navigation and returns the type the next segment is resolved on: the
  /// navigation's type, or its element type for a collection.
  /// </summary>
  private Type ValidateSegment(Type owner, MemberInfo member)
  {
    Type memberType = member switch
    {
      PropertyInfo property => property.PropertyType,
      FieldInfo field       => field.FieldType,
      _                     => throw new InvalidOperationException(
        $"'{owner.Name}.{member.Name}' cannot be included: it is not a property or a field.")
    };

    Type target = ElementType(memberType);

    if (target.IsValueType || target == typeof(string))
      throw new InvalidOperationException(
        $"'{owner.Name}.{member.Name}' cannot be included: it is a scalar value, not a navigation.");

    if (!owner.IsClass)
      throw new NotSupportedException(
        $"'{owner.Name}.{member.Name}' cannot be included: '{owner.Name}' is not a class, so the provider " +
        "cannot verify that the member is persisted in the document.");

    if (!BsonClassMap.LookupClassMap(owner).AllMemberMaps.Any(map => map.MemberName == member.Name))
      throw new NotSupportedException(
        $"'{owner.Name}.{member.Name}' cannot be included: it is not persisted in the document, " +
        "so there is nothing to load.");

    if (_model.TryGetEntity(target, out IEntityModel? entity))
      throw new NotSupportedException(
        $"'{owner.Name}.{member.Name}' cannot be included: '{target.Name}' is stored in its own collection " +
        $"'{entity.CollectionName}', and the MongoDB provider does not resolve references between collections. " +
        "Query that collection explicitly, passing the Session of the scope.");

    return target;
  }

  private static Type ElementType(Type type)
  {
    if (type == typeof(string))
      return type;

    if (type.IsArray)
      return type.GetElementType()!;

    Type? enumerable = IsEnumerable(type)
      ? type
      : type.GetInterfaces().FirstOrDefault(IsEnumerable);

    return enumerable?.GetGenericArguments()[0] ?? type;

    static bool IsEnumerable(Type candidate) =>
      candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IEnumerable<>);
  }

  private static InvalidOperationException InvalidExpression(Expression expression)
  {
    return new InvalidOperationException(
      $"The include expression '{expression}' is not valid. Use member accesses, such as 'x => x.Items', " +
      "'x => x.Details.Address' or 'x => new { x.Items, x.Details }'. Filtered includes are not supported.");
  }
}
