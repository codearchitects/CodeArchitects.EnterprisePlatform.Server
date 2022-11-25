using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationLeaf : INavigationLeaf
{
  public NavigationLeaf(INavigationModel model)
  {
    Model = model;
  }

  public int Index => Model.Id;

  public IEntityModel Target => Model.To;

  public INavigationModel Model { get; }

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
    return new EqualityVisitor(this).Visit(other);
  }

  private readonly struct EqualityVisitor : INavigationVisitor<bool>
  {
    private readonly INavigationLeaf _navigation;

    public EqualityVisitor(INavigationLeaf navigation)
    {
      _navigation = navigation;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Visit(INavigation navigation)
    {
      return navigation.Accept<EqualityVisitor, bool>(in this);
    }

    public readonly bool VisitLeaf(INavigationLeaf navigation)
    {
      return navigation.Index == _navigation.Index;
    }

    public readonly bool VisitNode(INavigationNode navigation)
    {
      return false;
    }
  }
}
