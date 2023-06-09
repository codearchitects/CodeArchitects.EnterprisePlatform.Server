using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class InitializerModel : IInitializerModel
{
  private readonly List<IAccessibleColumnModel> _constructorProperties;
  private readonly List<IAccessibleColumnModel> _initializerProperties;

  public InitializerModel(ConstructorInfo constructor)
  {
    Constructor = constructor;
    _constructorProperties = new();
    _initializerProperties = new();
  }

  public ConstructorInfo Constructor { get; }

  public IReadOnlyCollection<IAccessibleColumnModel> ConstructorProperties => _constructorProperties;

  public IReadOnlyCollection<IAccessibleColumnModel> InitializerProperties => _initializerProperties;

  public void AddConstructorProperty(IAccessibleColumnModel column) // TODO: The list of constructor properties is never checked against the constructor's parameters
  {
    _constructorProperties.Add(column);
  }

  public void AddInitializerProperty(IAccessibleColumnModel column)
  {
    _initializerProperties.Add(column);
  }

  public static InitializerModel Create(Type entityType)
  {
    ConstructorInfo constructor = entityType
      .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
      .OrderBy(ctor => ctor.GetParameters().Length)
      .First();

    return new(constructor);
  }
}
