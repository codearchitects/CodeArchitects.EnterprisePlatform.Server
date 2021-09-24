using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Data
{
  public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
  {
    IQueryable<TEntity> Query(params Expression<Func<TEntity, object?>>[] paths);
    TEntity? Find(TKey id, params Expression<Func<TEntity, object?>>[] paths);
    ValueTask<TEntity?> FindAsync(TKey id, Expression<Func<TEntity, object?>>[]? paths = null, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Add(TEntity entity);
    void Delete(TEntity entity);
  }
}
