using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationRoot
{
  NavigationCacheKey CacheKey { get; }
  IEntityModel Target { get; }
  IReadOnlyList<INavigation> Children { get; }
}
