using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

public partial class SqlTextBuilderTests
{
  private record NavigationLeaf(INavigationModel Model) : INavigationLeaf
  {
    public int Index => Model.Id;

    public IEntityModel Target => Model.To;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
      where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitLeaf(this);
    }

    public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
      where TVisitor : INavigationVisitor<TResult, TState>
    {
      return visitor.VisitLeaf(this, in state);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }

  private record NavigationNode(INavigationModel Model, IReadOnlyCollection<INavigation> Children) : INavigationNode
  {
    public int Index => Model.Id;

    public IEntityModel Target => Model.To;

    public TResult Accept<TVisitor, TResult>(in TVisitor visitor) where TVisitor : INavigationVisitor<TResult>
    {
      return visitor.VisitNode(this);
    }

    public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state) where TVisitor : INavigationVisitor<TResult, TState>
    {
      return visitor.VisitNode(this, in state);
    }

    public bool Equals(INavigation? other)
    {
      throw new NotImplementedException();
    }
  }
}
