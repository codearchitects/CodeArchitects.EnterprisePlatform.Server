using CodeArchitects.Platform.Data.Navigation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Navigation;

internal class CollectionIncluder<TParent, TEntity> : IExpressionIncluder<TEntity>
  where TParent : class
  where TEntity : class
{
  private readonly CollectionInclude<TParent, TEntity> _includeFunc;

  public CollectionIncluder(IQueryable<TParent> queryable, CollectionInclude<TParent, TEntity> includeFunc)
  {
    Queryable = queryable;
    _includeFunc = includeFunc;
  }

  public IQueryable<TParent> Queryable { get; private set; }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    Queryable = _includeFunc(Queryable).ThenInclude(includeExpression);

    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    ReferenceInclude<TParent, T> includeFunc = queryable => _includeFunc(queryable).ThenInclude(includeExpression)!;
    ReferenceIncluder<TParent, T> includer = new(Queryable, includeFunc);
    thenInclude(includer);

    Queryable = ReferenceEquals(Queryable, includer.Queryable)
      ? includeFunc(Queryable)
      : includer.Queryable;

    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    CollectionInclude<TParent, T> includeFunc = queryable => _includeFunc(queryable).ThenInclude(includeExpression)!;
    CollectionIncluder<TParent, T> includer = new(Queryable, includeFunc);
    thenInclude(includer);

    Queryable = ReferenceEquals(Queryable, includer.Queryable)
      ? includeFunc(Queryable)
      : includer.Queryable;

    return this;
  }
}
