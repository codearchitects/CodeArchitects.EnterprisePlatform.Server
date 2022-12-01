using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public Repository(IDataContext context)
  {
    Context = context;
  }

  protected IDataContext Context { get; }

  protected IDbConnection Connection => Context.Connection;

  public Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return Context.FindAsync<TEntity, TKey>(key, cancellationToken);
  }

  public Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    return Context.FindAsync(key, includeAction, cancellationToken);
  }

  public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.InsertAsync<TEntity, TKey>(entity, cancellationToken);
  }

  public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.UpdateAsync<TEntity, TKey>(entity, cancellationToken);
  }

  public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TEntity, TKey>(entity, cancellationToken);
  }

  public Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TEntity, TKey>(key, cancellationToken);
  }
}
