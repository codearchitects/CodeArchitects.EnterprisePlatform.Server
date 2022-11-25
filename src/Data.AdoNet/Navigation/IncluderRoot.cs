using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class IncluderRoot : IncluderNode
{
  public IncluderRoot(IEntityModel target)
  {
    Target = target;
  }

  public override IEntityModel Target { get; }
}
