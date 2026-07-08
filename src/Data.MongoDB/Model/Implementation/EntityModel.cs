using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

internal class EntityModel : IEntityModel
{
  public EntityModel(string tableName, Type type, IKeyModel key)
  {
    CollectionName = tableName;
    Type = type;
    Key = key;
  }

  public string CollectionName { get; }
  public Type Type { get; }
  public IKeyModel Key { get; }

  public static EntityModel Create(Type type)
  {
    string collectionName = type.GetCustomAttribute<TableAttribute>(inherit: false)?.Name ?? type.Name;
    PropertyInfo propertyInfo = GetIdPropertyInfo(type);

    KeyModel key = KeyModel.Create(propertyInfo);

    return new EntityModel(collectionName, type, key);
  }

  private static PropertyInfo GetIdPropertyInfo(Type type)
  {
    return type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public) ?? GetPropertyInfoFromAttribute(type);
  }

  private static PropertyInfo GetPropertyInfoFromAttribute(Type type)
  {
    try
    {
      return type
        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        .Single(property => property.IsDefined(typeof(BsonIdAttribute), inherit: false));
    }
    catch
    {
      throw new InvalidOperationException($"Cannot determine the id property of type '{type}'.");
    }
  }
}
