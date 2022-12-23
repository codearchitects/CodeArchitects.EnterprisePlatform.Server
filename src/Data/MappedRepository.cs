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

  protected abstract TEntity TableToEntity(TTable table);

  protected abstract TTable EntityToTable(TEntity entity);

  public virtual async Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    TTable? table = await DataContext.FindAsync<TTable, TKey>(key, cancellationToken);
    if (table is null)
      return null;

    return TableToEntity(table);
  }

  public virtual Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    throw new NotSupportedException();
  }

  public virtual async Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TTable table = EntityToTable(entity);
    await DataContext.InsertAsync<TTable, TKey>(table, cancellationToken);
  }

  public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TTable table = EntityToTable(entity);
    await DataContext.UpdateAsync<TTable, TKey>(table, cancellationToken);
  }

  public virtual async Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TTable table = EntityToTable(entity);
    await DataContext.RemoveAsync<TTable, TKey>(table, cancellationToken);
  }

  public virtual async Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    await DataContext.RemoveAsync<TTable, TKey>(key, cancellationToken);
  }
}
