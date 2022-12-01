using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public abstract class MappedRepository<TTable, TEntity, TKey> : IRepository<TEntity, TKey>
  where TTable : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private readonly IDataContext _context;

  public MappedRepository(IDataContext context)
  {
    _context = context;
  }

  protected DbContext DbContext => _context.DbContext;

  protected DbSet<TTable> Entities => _context.DbContext.Set<TTable>();

  protected abstract TTable EntityToTable(TEntity entity);

  protected abstract TEntity TableToEntity(TTable table);

  public virtual async Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    TTable? table = await _context.FindAsync<TTable, TKey>(key, cancellationToken);
    
    return table is not null
      ? TableToEntity(table)
      : null;
  }

  public virtual Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    throw new NotSupportedException();
  }

  public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TTable table = EntityToTable(entity);
    return _context.InsertAsync<TTable, TKey>(table, cancellationToken);
  }

  public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    TTable table = EntityToTable(entity);
    return _context.UpdateAsync<TTable, TKey>(table, cancellationToken);
  }

  public virtual Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return _context.RemoveAsync<TTable, TKey>(key, cancellationToken);
  }
}
