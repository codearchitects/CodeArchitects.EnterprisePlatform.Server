using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Materialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;

internal class DefaultEntityFactoryFactory : IDefaultEntityFactoryFactory
{
  private readonly IModel _model;

  public DefaultEntityFactoryFactory(IModel model)
  {
    _model = model;
  }

  public bool TryCreateFactory<TEntity, TKey>([NotNullWhen(true)] out Func<TKey, TEntity>? factory)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityType entityType = _model.FindEntityType(typeof(TEntity))!;
    if (!entityType.TryGetDefaultFactory(out Func<TKey, TEntity>? defaultFactory))
    {
      ConstructorInfo? constructor = typeof(TEntity).GetConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
        binder: null,
        types: Type.EmptyTypes,
        modifiers: null);

      ParameterExpression key = Expression.Parameter(typeof(TKey), nameof(key));
      if (constructor is not null && TryGetAssignments<TKey>(entityType, key, out IEnumerable<MemberAssignment>? assignments))
      {
        Expression<Func<TKey, TEntity>> factoryExpression = Expression.Lambda<Func<TKey, TEntity>>(
          body: Expression.MemberInit(
            newExpression: Expression.New(constructor),
            bindings: assignments),
          key);

        factory = factoryExpression.Compile();
        return true;
      }

      factory = null;
      return false;
    }

    factory = defaultFactory;
    return true;
  }

  private static bool TryGetAssignments<TKey>(IEntityType entityType, ParameterExpression key, [NotNullWhen(true)] out IEnumerable<MemberAssignment>? assignments)
    where TKey : IEquatable<TKey>
  {
    IReadOnlyList<IProperty> properties = entityType.FindPrimaryKey()!.Properties;
    MemberAssignment[] result = new MemberAssignment[properties.Count];

    if (typeof(ITuple).IsAssignableFrom(typeof(TKey)))
    {
      for (int i = 0; i < properties.Count; i++)
      {
        IProperty property = properties[i];
        MemberInfo? member = property.PropertyInfo ?? property.FieldInfo as MemberInfo;
        if (member is null)
        {
          assignments = null;
          return false;
        }

        result[i] = Expression.Bind(member, Expression.PropertyOrField(key, $"Item{i + 1}"));
      }
    }
    else
    {
      IProperty property = properties[0];
      MemberInfo? member = property.PropertyInfo ?? property.FieldInfo as MemberInfo;
      if (member is null)
      {
        assignments = null;
        return false;
      }

      result[0] = Expression.Bind(member, key);
    }

    assignments = result;
    return true;
  }
}
