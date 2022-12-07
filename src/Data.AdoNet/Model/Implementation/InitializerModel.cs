using CodeArchitects.Platform.Common.Reflection;
using CodeArchitects.Platform.Data.AdoNet.Model.Builder;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class InitializerModel : IInitializerModel
{
  private readonly IReadOnlyCollection<IAccessibleColumnModel> _constructorProperties;
  private readonly IReadOnlyCollection<IAccessibleColumnModel> _initializerProperties;

  public InitializerModel(ConstructorInfo constructor, IReadOnlyCollection<IAccessibleColumnModel> constructorProperties, IReadOnlyCollection<IAccessibleColumnModel> initializerProperties)
  {
    Constructor = constructor;
    _constructorProperties = constructorProperties;
    _initializerProperties = initializerProperties;
  }

  public ConstructorInfo Constructor { get; }

  public IReadOnlyCollection<IAccessibleColumnModel> ConstructorProperties => _constructorProperties;

  public IReadOnlyCollection<IAccessibleColumnModel> InitializerProperties => _initializerProperties;

  public static InitializerModel Create(Type type, IReadOnlyList<IAccessibleColumnModel> columns)
  {
    ConstructorInfo constructor = type
      .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
      .OrderBy(ctor => ctor.GetParameters().Length)
      .First();

    HashSet<IAccessibleColumnModel> constructorProperties = new();
    foreach (ParameterInfo parameter in constructor.GetParameters())
    {
      if (parameter.Name is null)
        throw new ModelConfigurationException($"Found nameless constructor parameter in type '{type.Name}'.");

      IAccessibleColumnModel column = columns.SingleOrDefault(col => col.Member.Name.MatchesCamelCaseConvention(parameter.Name))
        ?? throw new ModelConfigurationException($"Could not resolve a member of type '{type.Name}' corresponding to parameter named '{parameter.Name}' by convention.");

      constructorProperties.Add(column);
    }

    List<IAccessibleColumnModel> initializerProperties = columns
      .Where(col => !constructorProperties.Contains(col))
      .ToList();

    return new(constructor, constructorProperties, initializerProperties);
  }
}
