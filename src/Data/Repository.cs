using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data;

/// <summary>
/// Base implementation of <see cref="IRepository{TEntity, TKey}"/>.
/// </summary>
/// <remarks>
/// Do not use this class directly.
/// This class is meant to be extended by specific implementations which will internally use some ORM to perform the operations.
/// </remarks>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public abstract class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private protected abstract IDataContext DataContext { get; }

  /// <inheritdoc/>
  public virtual TEntity? Find(TKey key)
  {
    return DataContext.Find<TEntity, TKey>(key);
  }

  /// <inheritdoc/>
  public virtual async Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return await DataContext.FindAsync<TEntity, TKey>(key, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual TEntity? Find(TKey key, IncludeAction<TEntity> includeAction)
  {
    return DataContext.Find(key, includeAction);
  }

  /// <inheritdoc/>
  public virtual async Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    return await DataContext.FindAsync(key, includeAction, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void Insert(TEntity entity)
  {
    DataContext.Insert<TEntity, TKey>(entity);
  }

  /// <inheritdoc/>
  public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await DataContext.InsertAsync<TEntity, TKey>(entity, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void InsertMany(IEnumerable<TEntity> entities)
  {
    DataContext.InsertMany<TEntity, TKey>(entities);
  }

  /// <inheritdoc/>
  public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    await DataContext.InsertManyAsync<TEntity, TKey>(entities, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void Update(TEntity entity)
  {
    DataContext.Update<TEntity, TKey>(entity);
  }

  /// <inheritdoc/>
  public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await DataContext.UpdateAsync<TEntity, TKey>(entity, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void UpdateMany(IEnumerable<TEntity> entities)
  {
    DataContext.UpdateMany<TEntity, TKey>(entities);
  }

  /// <inheritdoc/>
  public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    await DataContext.UpdateManyAsync<TEntity, TKey>(entities, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void Upsert(TEntity entity)
  {
    DataContext.Upsert<TEntity, TKey>(entity);
  }

  /// <inheritdoc/>
  public virtual async Task UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await DataContext.UpsertAsync<TEntity, TKey>(entity, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void Remove(TEntity entity)
  {
    DataContext.Remove<TEntity, TKey>(entity);
  }

  /// <inheritdoc/>
  public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await DataContext.RemoveAsync<TEntity, TKey>(entity, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void Remove(TKey key)
  {
    DataContext.Remove<TEntity, TKey>(key);
  }

  /// <inheritdoc/>
  public virtual async Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    await DataContext.RemoveAsync<TEntity, TKey>(key, cancellationToken);
  }
}
