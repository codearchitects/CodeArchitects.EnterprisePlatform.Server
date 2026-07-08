using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class SimpleNavigationLeaf : ISimpleNavigationLeaf
{
  public SimpleNavigationLeaf(IAccessibleSimpleNavigationModel model)
  {
    Model = model;
  }

  public IAccessibleSimpleNavigationModel Model { get; }

  IAccessibleNavigationModel INavigation.Model => Model;

  public IEntityModel Target => Model.To;

  public IReadOnlyCollection<INavigation> Children => Array.Empty<INavigation>();

  public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
    where TVisitor : INavigationVisitor<TResult>
  {
    return visitor.VisitSimpleLeaf(this);
  }
}
