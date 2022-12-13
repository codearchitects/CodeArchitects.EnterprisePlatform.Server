using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

public partial class SqlTextBuilderTests
{
  private record SimpleNavigationLeaf(IAccessibleSimpleNavigationModel Model) : ISimpleNavigationLeaf
  {
    public IEntityModel Target => Model.To;

    public IReadOnlyCollection<INavigation> Children => Array.Empty<INavigation>();

    IAccessibleNavigationModel INavigation.Model => Model;

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
      throw new NotImplementedException();
    }
  }

  private record SkipNavigationLeaf(IAccessibleSkipNavigationModel Model) : ISkipNavigationLeaf
  {
    public IEntityModel Target => Model.To;

    public IReadOnlyCollection<INavigation> Children => Array.Empty<INavigation>();

    IAccessibleNavigationModel INavigation.Model => Model;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
      where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitSkipLeaf(this);
    }

    public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
      where TVisitor : INavigationVisitor<TResult, TState>
    {
      return visitor.VisitSkipLeaf(this, in state);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }

  private record SimpleNavigationNode(IAccessibleSimpleNavigationModel Model, IReadOnlyCollection<INavigation> Children) : ISimpleNavigationNode
  {
    public IEntityModel Target => Model.To;

    IAccessibleNavigationModel INavigation.Model => Model;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor) where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitSimpleNode(this);
    }

    public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state) where TVisitor : INavigationVisitor<TResult, TState>
    {
      return visitor.VisitSimpleNode(this, in state);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }

  private record SkipNavigationNode(IAccessibleSkipNavigationModel Model, IReadOnlyCollection<INavigation> Children) : ISkipNavigationNode
  {
    public IEntityModel Target => Model.To;

    IAccessibleSkipNavigationModel ISkipNavigationNode.Model => Model;

    IAccessibleNavigationModel INavigation.Model => Model;

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
      throw new NotImplementedException();
    }
  }
}
