using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class SkipNavigationLeaf : ISkipNavigationLeaf
{
  public SkipNavigationLeaf(IAccessibleSkipNavigationModel model)
  {
    Model = model;
  }

  public IAccessibleSkipNavigationModel Model { get; }

  IAccessibleNavigationModel INavigation.Model => Model;

  public IEntityModel Target => Model.To;

  public IReadOnlyCollection<INavigation> Children => Array.Empty<INavigation>();

  public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
    where TVisitor : INavigationVisitor<TResult>
  {
    return visitor.VisitSkipLeaf(this);
  }

  public bool Equals(INavigation? other)
  {
    if (other is not ISkipNavigationLeaf leaf)
      return false;

    return Model.Id == leaf.Model.Id;
  }
}
