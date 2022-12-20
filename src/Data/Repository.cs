namespace CodeArchitects.Platform.Data;

/// <summary>
/// Base implementation of <see cref="IRepository{TEntity, TKey}"/>.
/// </summary>
/// <remarks>
/// This class is meant to be extended by specific implementations which will internally use some ORM to perform the operations.
/// </remarks>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public abstract class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// The data context used by the repository.
  /// </summary>
  protected abstract IDataContext Context { get; }

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

  public virtual Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TEntity, TKey>(entity, cancellationToken);
  }

  public virtual Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TEntity, TKey>(key, cancellationToken);
  }
}
