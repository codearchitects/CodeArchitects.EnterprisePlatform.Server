using MongoDB.Bson.Serialization;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

/// <summary>
/// Keeps the key of a domain entity and the key of the document it is mapped to in step.
/// </summary>
internal static class MappedKey<TDocument, TEntity, TKey>
  where TDocument : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private static readonly Lazy<EntityKey?> s_entityKey = new(ResolveEntityKey);

  /// <summary>
  /// Throws when the mapping produced a document whose key differs from the entity key.
  /// </summary>
  public static void EnsurePreserved(IEntityModel documentModel, TEntity entity, TDocument document, string? position)
  {
    if (s_entityKey.Value is not EntityKey entityKey || !TryGetKey(entityKey, entity, out TKey key))
      return;

    TKey documentKey = GetDocumentKey(documentModel, document);
    if (key.Equals(documentKey))
      return;

    throw new InvalidOperationException(
      $"The mapping from '{typeof(TEntity).Name}' to '{typeof(TDocument).Name}'{position} does not preserve the key: " +
      $"the entity has '{key}', the document has '{documentKey}'. Map '{typeof(TEntity).Name}.{entityKey.Property.Name}' " +
      $"to '{typeof(TDocument).Name}.{documentModel.Key.Name}'.");
  }

  /// <summary>
  /// Before an insert: generates the document key when it is empty, as the driver would.
  /// </summary>
  public static void GenerateKey(IEntityModel documentModel, TDocument document)
  {
    BsonMemberMap? idMemberMap = BsonClassMap.LookupClassMap(typeof(TDocument)).IdMemberMap;

    // The driver only generates the key of the member it maps to _id: when the provider resolved
    // a different member (the <TypeName>Id convention), there is nothing the driver would do.
    if (idMemberMap?.IdGenerator is null || idMemberMap.MemberName != documentModel.Key.Name)
      return;

    object? id = idMemberMap.Getter(document);
    if (idMemberMap.IdGenerator.IsEmpty(id))
    {
      idMemberMap.Setter(document, idMemberMap.IdGenerator.GenerateId(container: null, document));
    }
  }

  /// <summary>
  /// After the data context accepted an insert: writes the document key back to the entity when the entity has none.
  /// </summary>
  public static void AssignKey(IEntityModel documentModel, TEntity entity, TDocument document)
  {
    if (s_entityKey.Value is not EntityKey { Setter: not null } entityKey || TryGetKey(entityKey, entity, out _))
      return;

    TKey documentKey = GetDocumentKey(documentModel, document);
    if (!IsDefault(documentKey))
    {
      entityKey.Setter(entity, documentKey);
    }
  }

  private static TKey GetDocumentKey(IEntityModel documentModel, TDocument document) =>
    KeyAccessor.Get<TDocument, TKey>(documentModel, document);

  /// <returns><c>true</c> when the entity has a key that is not the default value.</returns>
  private static bool TryGetKey(EntityKey entityKey, TEntity entity, out TKey key)
  {
    object? value = entityKey.Getter(entity);
    key = value is null ? default! : (TKey)value;

    return value is not null && !IsDefault(key);
  }

  private static bool IsDefault(TKey key) => key is null || EqualityComparer<TKey>.Default.Equals(key, default!);

  private static EntityKey? ResolveEntityKey()
  {
    Type type = typeof(TEntity);
    PropertyInfo? property = FindKey(type, "Id") ?? FindKey(type, type.Name + "Id");
    if (property is null)
      return null;

    ParameterExpression parameter = Expression.Parameter(type, "entity");
    Func<TEntity, object?> getter = Expression.Lambda<Func<TEntity, object?>>(
      Expression.Convert(Expression.Property(parameter, property), typeof(object)), parameter).Compile();

    // A private setter declared in a base class is only visible from the declaring type.
    PropertyInfo declared = property.DeclaringType!.GetProperty(
      property.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)!;

    // Only used on insert, when the key was generated: reflection keeps private and init setters working.
    Action<TEntity, TKey>? setter = declared.GetSetMethod(nonPublic: true) is null
      ? null
      : (entity, key) => declared.SetValue(entity, key);

    return new EntityKey(property, getter, setter);
  }

  private static PropertyInfo? FindKey(Type type, string name)
  {
    PropertyInfo? property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
    if (property is null || !property.CanRead)
      return null;

    Type propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
    return propertyType == typeof(TKey) ? property : null;
  }

  private sealed class EntityKey(PropertyInfo property, Func<TEntity, object?> getter, Action<TEntity, TKey>? setter)
  {
    public PropertyInfo Property { get; } = property;

    public Func<TEntity, object?> Getter { get; } = getter;

    public Action<TEntity, TKey>? Setter { get; } = setter;
  }
}
