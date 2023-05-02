using CodeArchitects.Platform.Data.Navigation;
using System.Threading;

namespace CodeArchitects.Platform.Data;

/// <summary>
/// Base implementation of <see cref="IRepository{TEntity, TKey}"/> which performs mapping between table entities and domain entities.
/// </summary>
/// <remarks>
/// Do not use this class directly.
/// This class is meant to be extended by specific implementations which will internally use some ORM to perform the operations.
/// </remarks>
/// <typeparam name="TTable">The table entity type.</typeparam>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
/// <typeparam name="TKey">The entity's primary key type.</typeparam>
public abstract class MappedRepository<TTable, TEntity, TKey> : IRepository<TEntity, TKey>
  where TTable : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private protected abstract IDataContext DataContext { get; }

  /// <summary>
  /// Maps a table entity to a domain entity.
  /// </summary>
  /// <param name="table">The table entity.</param>
  /// <returns>The domain entity.</returns>
  protected abstract TEntity TableToEntity(TTable table);

  /// <summary>
  /// Maps a domain entity to a table entity.
  /// </summary>
  /// <param name="entity">The domain entity.</param>
  /// <returns>The table entity.</returns>
  protected abstract TTable EntityToTable(TEntity entity);

  /// <inheritdoc/>
  public virtual TEntity? Find(TKey key)
  {
    TTable? table = DataContext.Find<TTable, TKey>(key);
    if (table is null)
      return null;

    return TableToEntity(table);
  }

  /// <inheritdoc/>
  public virtual async Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    TTable? table = await DataContext.FindAsync<TTable, TKey>(key, cancellationToken);
    if (table is null)
      return null;

    return TableToEntity(table);
  }

  /// <inheritdoc/>
  public virtual TEntity? Find(TKey key, IncludeAction<TEntity> includeAction)
  {
    throw new NotSupportedException();
  }

  /// <inheritdoc/>
  public virtual Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    throw new NotSupportedException();
  }

  /// <inheritdoc/>
  public virtual void Insert(TEntity entity)
  {
    TTable table = EntityToTable(entity);
    DataContext.Insert<TTable, TKey>(table);
  }

  /// <inheritdoc/>
  public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TTable table = EntityToTable(entity);
    await DataContext.InsertAsync<TTable, TKey>(table, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void InsertMany(IEnumerable<TEntity> entities)
  {
    IEnumerable<TTable> tables = entities.Select(EntityToTable);
    DataContext.InsertMany<TTable, TKey>(tables);
  }

  /// <inheritdoc/>
  public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    IEnumerable<TTable> tables = entities.Select(EntityToTable);
    await DataContext.InsertManyAsync<TTable, TKey>(tables, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void Update(TEntity entity)
  {
    TTable table = EntityToTable(entity);
    DataContext.Update<TTable, TKey>(table);
  }

  /// <inheritdoc/>
  public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TTable table = EntityToTable(entity);
    await DataContext.UpdateAsync<TTable, TKey>(table, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void UpdateMany(IEnumerable<TEntity> entities)
  {
    IEnumerable<TTable> tables = entities.Select(EntityToTable);
    DataContext.UpdateMany<TTable, TKey>(tables);
  }

  /// <inheritdoc/>
  public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    IEnumerable<TTable> tables = entities.Select(EntityToTable);
    await DataContext.UpdateManyAsync<TTable, TKey>(tables, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void Upsert(TEntity entity)
  {
    TTable table = EntityToTable(entity);
    DataContext.Upsert<TTable, TKey>(table);
  }

  /// <inheritdoc/>
  public virtual async Task UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TTable table = EntityToTable(entity);
    await DataContext.UpsertAsync<TTable, TKey>(table, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void Remove(TEntity entity)
  {
    TTable table = EntityToTable(entity);
    DataContext.Remove<TTable, TKey>(table);
  }

  /// <inheritdoc/>
  public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TTable table = EntityToTable(entity);
    await DataContext.RemoveAsync<TTable, TKey>(table, cancellationToken);
  }

  /// <inheritdoc/>
  public virtual void Remove(TKey key)
  {
    DataContext.Remove<TTable, TKey>(key);
  }

  /// <inheritdoc/>
  public virtual async Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    await DataContext.RemoveAsync<TTable, TKey>(key, cancellationToken);
  }
}
