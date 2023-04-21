using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

internal class KeyModel : IKeyModel
{
  public KeyModel(string name, Type type)
  {
    Name = name;
    Type = type;
  }

  public string Name { get; }

  public Type Type { get; }

  public static KeyModel Create(PropertyInfo propertyInfo)
  {
    return new KeyModel(propertyInfo.Name, propertyInfo.PropertyType);
  }
}
