using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationRoot : IncluderNode
{
  public NavigationRoot(IEntityModel target)
  {
    Target = target;
  }

  public override IEntityModel Target { get; }
}
