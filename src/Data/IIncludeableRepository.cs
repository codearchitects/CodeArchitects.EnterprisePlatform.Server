using System;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data
{
  public interface IIncludeableRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
  {
    IRepository<TEntity, TKey> Include(params Expression<Func<TEntity, object?>>[] paths);
  }
}
