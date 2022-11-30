using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private readonly IDataContext _context;

  public Repository(IDataContext context)
  {
    _context = context;
  }

  protected DbContext DbContext => _context.DbContext;

  protected DbSet<TEntity> Entities => _context.DbContext.Set<TEntity>();

  public virtual Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return _context.FindAsync<TEntity, TKey>(key, cancellationToken);
  }

  public virtual Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    return _context.FindAsync(key, includeAction, cancellationToken);
  }

  public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return _context.InsertAsync<TEntity, TKey>(entity, cancellationToken);
  }

  public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return _context.UpdateAsync<TEntity, TKey>(entity, cancellationToken);
  }

  public virtual Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    return _context.RemoveAsync<TEntity, TKey>(key, cancellationToken);
  }

  public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    return _context.RemoveAsync<TEntity, TKey>(entity, cancellationToken);
  }
}
