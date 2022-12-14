using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public abstract class EFCoreMappedRepository<TTable, TEntity, TKey> : IRepository<TEntity, TKey>
  where TTable : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public EFCoreMappedRepository(IDataContext context)
  {
    Context = context;
  }

  protected IDataContext Context { get; }

  protected DbContext DbContext => Context.DbContext;

  protected DbSet<TTable> Entities => DbContext.Set<TTable>();

  protected abstract TEntity TableToEntity(TTable table);

  protected abstract TTable EntityToTable(TEntity entity);

  public Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }

  public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
