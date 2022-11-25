using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationNode : IncluderNode, INavigationNode
{
  public NavigationNode(INavigationModel model)
  {
    Model = model;
  }

  public int Index => Model.Id;

  public INavigationModel Model { get; }

  public override IEntityModel Target => Model.To;

  public TResult Accept<TVisitor, TResult>(in TVisitor visitor)
    where TVisitor : INavigationVisitor<TResult>
  {
    return visitor.VisitNode(this);
  }

  public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state)
    where TVisitor : INavigationVisitor<TResult, TState>
  {
    return visitor.VisitNode(this, in state);
  }

  public bool Equals(INavigation other)
  {
    return new EqualityVisitor(this).Visit(other);
  }

  private readonly struct EqualityVisitor : INavigationVisitor<bool>
  {
    private readonly INavigationNode _navigation;

    public EqualityVisitor(INavigationNode navigation)
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
      return false;
    }

    public readonly bool VisitNode(INavigationNode navigation)
    {
      return
        navigation.Index == _navigation.Index &&
        NavigationCollection.Equal(navigation.Children, _navigation.Children);
    }
  }
}
