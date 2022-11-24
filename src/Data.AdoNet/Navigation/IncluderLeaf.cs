using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class IncluderLeaf : INavigationLeaf
{
  private readonly INavigationModel _navigation;

  public IncluderLeaf(INavigationModel navigation)
  {
    _navigation = navigation;
  }

  public int Index => _navigation.Id;

  public IEntityModel Target => _navigation.To;

  public INavigationModel Model => _navigation;

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

  public bool Equals(INavigation other)
  {
    return new EqualityVisitor(Index).Visit(other);
  }

  private readonly struct EqualityVisitor : INavigationVisitor<bool>
  {
    private readonly int _index;

    public EqualityVisitor(int index)
    {
      _index = index;
    }

    public readonly bool Visit(INavigation navigation)
    {
      return navigation.Accept<EqualityVisitor, bool>(in this);
    }

    public readonly bool VisitLeaf(INavigationLeaf navigation)
    {
      return navigation.Index == _index;
    }

    public readonly bool VisitNode(INavigationNode navigation)
    {
      return false;
    }
  }
}
