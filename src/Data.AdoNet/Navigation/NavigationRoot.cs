using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationRoot : IncluderNode, INavigationRoot
{
  public NavigationRoot(IEntityModel target)
  {
    Target = target;
  }

  public override IEntityModel Target { get; }

  public IEntityModel Entity => Target;

  public IReadOnlyCollection<INavigation> Navigations => Children;
}
