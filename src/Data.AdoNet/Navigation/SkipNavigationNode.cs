using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class SkipNavigationNode : NavigationNode, ISkipNavigationNode
{
  public SkipNavigationNode(IAccessibleSkipNavigationModel model)
  {
    Model = model;
  }

  public IAccessibleSkipNavigationModel Model { get; }

  IAccessibleNavigationModel INavigation.Model => Model;

  public override IEntityModel Target => Model.To;

  public TResult Accept<TVisitor, TResult>(in TVisitor visitor) where TVisitor : INavigationVisitor<TResult>
  {
    return visitor.VisitSkipNode(this);
  }
}
