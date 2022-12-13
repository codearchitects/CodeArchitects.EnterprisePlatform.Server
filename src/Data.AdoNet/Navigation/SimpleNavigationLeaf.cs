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

  public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
    where TVisitor : INavigationVisitor<TResult, TState>
  {
    return visitor.VisitSimpleLeaf(this, in state);
  }

  public bool Equals(INavigation? other)
  {
    if (other is not ISimpleNavigationLeaf leaf)
      return false;

    return Model.Id == leaf.Model.Id;
  }
}
