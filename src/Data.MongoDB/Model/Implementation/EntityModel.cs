using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

internal class EntityModel(string tableName, Type type, IKeyModel key) : IEntityModel
{
  private static readonly HashSet<Type> s_supportedKeyTypes =
  [
    typeof(Guid), typeof(string), typeof(ObjectId), typeof(int), typeof(long)
  ];

  public string CollectionName { get; } = tableName;
  public Type Type { get; } = type;
  public IKeyModel Key { get; } = key;

  public static EntityModel Create(Type type)
  {
    string collectionName = type.GetCustomAttribute<CollectionAttribute>(inherit: false)?.Name
      ?? type.GetCustomAttribute<TableAttribute>(inherit: false)?.Name
      ?? type.Name;

    PropertyInfo key = ResolveKey(type);

    if (!s_supportedKeyTypes.Contains(Nullable.GetUnderlyingType(key.PropertyType) ?? key.PropertyType))
      throw new InvalidOperationException($"The type of the id property '{key.Name}' in type '{type}' is not supported. Supported types are: {string.Join(", ", s_supportedKeyTypes.Select(t => t.Name))}.");

    return new EntityModel(collectionName, type, KeyModel.Create(key));
  }

  private static PropertyInfo ResolveKey(Type type)
  {
    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    PropertyInfo[] bsonId = [.. properties.Where(p => p.IsDefined(typeof(BsonIdAttribute), inherit: false))];

    if (bsonId.Length > 1)
      throw new InvalidOperationException($"Type '{type}' has multiple properties marked with [BsonId].");

    if (bsonId.Length == 1)
      return bsonId[0];

    return properties.FirstOrDefault(p => p.Name == "Id")
      ?? properties.FirstOrDefault(p => p.Name == type.Name + "Id")
      ?? throw new InvalidOperationException($"Cannot determine the id property of type '{type}'.");
  }
}
