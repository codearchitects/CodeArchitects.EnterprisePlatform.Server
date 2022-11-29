using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationSkipNode : IncluderNode, INavigationSkipNode
{
  public NavigationSkipNode(ISkipNavigationModel model)
  {
    Model = model;
  }

  public ISkipNavigationModel Model { get; }

  INavigationModel INavigation.Model => Model;

  public override IEntityModel Target => Model.To;

  public TResult Accept<TVisitor, TResult>(in TVisitor visitor) where TVisitor : INavigationVisitor<TResult>
  {
    return visitor.VisitSkipNode(this);
  }

  public TResult Accept<TVisitor, TResult, TState>(in TVisitor visitor, in TState state) where TVisitor : INavigationVisitor<TResult, TState>
  {
    return visitor.VisitSkipNode(this, in state);
  }

  public bool Equals(INavigation other)
  {
    return new EqualityVisitor(this).Visit(other);
  }

  private readonly struct EqualityVisitor : INavigationVisitor<bool>
  {
    private readonly INavigationSkipNode _navigation;

    public EqualityVisitor(INavigationSkipNode navigation)
    {
      _navigation = navigation;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Visit(INavigation navigation)
    {
      return navigation.Accept<EqualityVisitor, bool>(in this);
    }

    public readonly bool VisitSimpleLeaf(INavigationSimpleLeaf navigation)
    {
      return false;
    }

    public readonly bool VisitSimpleNode(INavigationSimpleNode navigation)
    {
      return false;
    }

    public bool VisitSkipLeaf(INavigationSkipLeaf navigation)
    {
      return false;
    }

    public bool VisitSkipNode(INavigationSkipNode navigation)
    {
      return
        navigation.Model.Id == _navigation.Model.Id &&
        NavigationCollection.Equal(navigation.Children, _navigation.Children);
    }
  }
}
