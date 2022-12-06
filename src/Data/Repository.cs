namespace CodeArchitects.Platform.Data;

public abstract class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  protected abstract IDataContext Context { get; }

  protected virtual string EntityName => typeof(TEntity).Name;

  public virtual Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return Context.FindAsync<TEntity, TKey>(EntityName, key, cancellationToken);
  }

  public virtual Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    return Context.FindAsync(EntityName, key, includeAction, cancellationToken);
  }

  public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.InsertAsync<TEntity, TKey>(EntityName, entity, cancellationToken);
  }

  public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.UpdateAsync<TEntity, TKey>(EntityName, entity, cancellationToken);
  }

  public virtual Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TEntity, TKey>(EntityName, entity, cancellationToken);
  }

  public virtual Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TEntity, TKey>(EntityName, key, cancellationToken);
  }
}
