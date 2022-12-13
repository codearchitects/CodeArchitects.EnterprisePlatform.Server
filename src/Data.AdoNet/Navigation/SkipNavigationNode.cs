using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class SkipNavigationNode : IncluderNode, ISkipNavigationNode
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

  public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state) where TVisitor : INavigationVisitor<TResult, TState>
  {
    return visitor.VisitSkipNode(this, in state);
  }

  public bool Equals(INavigation? other)
  {
    if (other is not INavigationNode node)
      return false;

    return
      Model.Id == node.Model.Id &&
      NavigationCollection.Equal(Children, node.Children);
  }
}
