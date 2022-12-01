using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet;

public abstract class MappedRepository<TTable, TEntity, TKey> : IRepository<TEntity, TKey>
  where TTable : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public MappedRepository(IDataContext context)
  {
    Context = context;
  }

  protected IDataContext Context { get; }

  protected IDbConnection Connection => Context.Connection;

  protected abstract TTable EntityToTable(TEntity entity);

  protected abstract TEntity TableToEntity(TTable table);

  public virtual async Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    TTable? table = await Context.FindAsync<TTable, TKey>(key, cancellationToken);
    if (table is null)
      return null;

    return TableToEntity(table);
  }

  public virtual Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    throw new NotSupportedException();
  }

  public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.InsertAsync<TTable, TKey>(EntityToTable(entity), cancellationToken);
  }

  public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.UpdateAsync<TTable, TKey>(EntityToTable(entity), cancellationToken);
  }

  public virtual Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TTable, TKey>(key, cancellationToken);
  }

  public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return Context.RemoveAsync<TTable, TKey>(EntityToTable(entity), cancellationToken);
  }
}
