using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Data
{
  public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
  {
    IQueryable<TEntity> Query();
    TEntity? Find(TKey id);
    ValueTask<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Add(TEntity entity);
    void Delete(TEntity entity);
  }
}
