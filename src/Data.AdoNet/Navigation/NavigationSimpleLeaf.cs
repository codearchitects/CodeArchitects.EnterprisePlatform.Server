using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationSimpleLeaf : INavigationSimpleLeaf
{
  public NavigationSimpleLeaf(IAccessibleSimpleNavigationModel model)
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
    return other is not null && new EqualityVisitor(this).Visit(other);
  }

  private readonly struct EqualityVisitor : INavigationVisitor<bool>
  {
    private readonly INavigationSimpleLeaf _navigation;

    public EqualityVisitor(INavigationSimpleLeaf navigation)
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
      return navigation.Model.Id == _navigation.Model.Id;
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
      return false;
    }
  }
}
