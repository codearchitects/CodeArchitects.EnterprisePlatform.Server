using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class SimpleNavigationNode : NavigationNode, ISimpleNavigationNode
{
  public SimpleNavigationNode(IAccessibleSimpleNavigationModel model)
  {
    Model = model;
  }

  public IAccessibleSimpleNavigationModel Model { get; }

  IAccessibleNavigationModel INavigation.Model => Model;

  public override IEntityModel Target => Model.To;

  public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
    where TVisitor : INavigationVisitor<TResult>
  {
    return visitor.VisitSimpleNode(this);
  }
}
