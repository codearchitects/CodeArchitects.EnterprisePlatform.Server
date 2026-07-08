using CodeArchitects.Platform.Data.Navigation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Navigation;

internal class Includer<TEntity> : IIncluder<TEntity>
  where TEntity : class
{
  public Includer(IQueryable<TEntity> queryable)
  {
    Queryable = queryable;
  }

  public IQueryable<TEntity> Queryable { get; private set; }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    Queryable = Queryable.Include(includeExpression);

    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    if (thenInclude is null)
      throw new ArgumentNullException(nameof(thenInclude));

    ReferenceInclude<TEntity, T> includeFunc = queryable => queryable.Include(includeExpression)!;
    ReferenceIncluder<TEntity, T> includer = new(Queryable, includeFunc);
    thenInclude(includer);

    Queryable = ReferenceEquals(Queryable, includer.Queryable)
      ? includeFunc(Queryable)
      : includer.Queryable;

    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    if (thenInclude is null)
      throw new ArgumentNullException(nameof(thenInclude));

    CollectionInclude<TEntity, T> includeFunc = queryable => queryable.Include(includeExpression)!;
    CollectionIncluder<TEntity, T> includer = new(Queryable, includeFunc);
    thenInclude(includer);

    Queryable = ReferenceEquals(Queryable, includer.Queryable)
      ? includeFunc(Queryable)
      : includer.Queryable;

    return this;
  }

  public IStringIncluder<TEntity> Include(string navigation)
  {
    Queryable = Queryable.Include(navigation);
    return this;
  }
}
