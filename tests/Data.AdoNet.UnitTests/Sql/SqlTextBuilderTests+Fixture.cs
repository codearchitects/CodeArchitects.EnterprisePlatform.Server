using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

public partial class SqlTextBuilderTests
{
  private record NavigationLeaf(INavigationModel Model) : INavigationLeaf
  {
    public int Id => Model.Id;

    public IEntityModel Target => Model.To;

    public IReadOnlyCollection<INavigation> Children => Array.Empty<INavigation>();

    INavigationModelBase INavigation.Model => Model;

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

  private record NavigationSkipLeaf(ISkipNavigationModel Model) : INavigationSkipLeaf
  {
    public int Id => Model.Id;

    public IEntityModel Target => Model.To;

    public IReadOnlyCollection<INavigation> Children => Array.Empty<INavigation>();

    INavigationModelBase INavigation.Model => Model;

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

  private record NavigationNode(INavigationModel Model, IReadOnlyCollection<INavigation> Children) : INavigationNode
  {
    public int Id => Model.Id;

    public IEntityModel Target => Model.To;

    INavigationModelBase INavigation.Model => Model;

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

  private record NavigationSkipNode(ISkipNavigationModel Model, IReadOnlyCollection<INavigation> Children) : INavigationSkipNode
  {
    public int Id => Model.Id;

    public IEntityModel Target => Model.To;

    INavigationModelBase INavigation.Model => Model;

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
