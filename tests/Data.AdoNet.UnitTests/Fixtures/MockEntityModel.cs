using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Fixtures;

internal class MockEntityModel : IEntityModel
{
  private readonly IEntityModel _mock;

  public MockEntityModel(IEntityModel mock)
  {
    _mock = mock;
  }

  public string Name => _mock.Name;

  public string TableName => _mock.TableName;

  public Type Type => _mock.Type;

  public IPrimaryKeyModel PrimaryKey => _mock.PrimaryKey;

  public IReadOnlyList<IPropertyModel> Properties => _mock.Properties;

  public IReadOnlyList<INavigationModel> Navigations => _mock.Navigations;

  public IInitializerModel Initializer => _mock.Initializer;

  public bool TryGetNavigation(ReadOnlySpan<char> name, [NotNullWhen(true)] out INavigationModel? navigationModel)
  {
    foreach (INavigationModel navigation in Navigations)
    {
      if (navigation.Name == name.ToString())
      {
        navigationModel = navigation;
        return true;
      }
    }

    navigationModel = null;
    return false;
  }
}

internal static class MockEntityModelExtensions
{
  public static IEntityModel Mocked(this IEntityModel mock)
  {
    return new MockEntityModel(mock);
  }
}