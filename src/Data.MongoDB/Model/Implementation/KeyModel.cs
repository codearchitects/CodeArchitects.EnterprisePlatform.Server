using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

internal class KeyModel(string name, Type type) : IKeyModel
{
  public const string IdElementName = "_id";

  public string Name { get; } = name;

  public Type Type { get; } = type;

  public string ElementName => IdElementName;

  public static KeyModel Create(PropertyInfo propertyInfo)
  {
    return new KeyModel(propertyInfo.Name, propertyInfo.PropertyType);
  }
}
