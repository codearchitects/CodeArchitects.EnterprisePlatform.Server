using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public Repository(IDataContext context)
  {
    Context = context;
  }

  protected IDataContext Context { get; }

  protected DbContext DbContext => Context.DbContext;

  protected DbSet<TEntity> Entities => Context.DbContext.Set<TEntity>();

  public virtual Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return Context.FindAsync<TEntity, TKey>(key, cancellationToken);
  }

  public virtual Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    return Context.FindAsync(key, includeAction, cancellationToken);
  }

  public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.InsertAsync<TEntity, TKey>(entity, cancellationToken);
  }

  public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.UpdateAsync<TEntity, TKey>(entity, cancellationToken);
  }

  public virtual Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TEntity, TKey>(key, cancellationToken);
  }

  public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TEntity, TKey>(entity, cancellationToken);
  }
}
