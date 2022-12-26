using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationRoot
{
  IEntityModel Entity { get; }
  IReadOnlyCollection<INavigation> Navigations { get; }
}
