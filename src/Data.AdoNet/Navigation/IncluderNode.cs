using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class IncluderNode<TEntity> : IExpressionIncluder<TEntity>, INavigationNode
  where TEntity : class
{
  private readonly Dictionary<int, INavigation> _navigations;

  public IncluderNode(INavigationModel model)
  {
    _navigations = new();
    Model = model;
  }

  public IReadOnlyCollection<INavigation> Children => _navigations.Values;

  public int Index => Model.Id;

  public IEntityModel Target => Model.To;

  public INavigationModel Model { get; }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    throw new NotImplementedException();
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    throw new NotImplementedException();
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    throw new NotImplementedException();
  }

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
