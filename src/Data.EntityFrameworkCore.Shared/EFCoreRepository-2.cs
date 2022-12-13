using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public abstract class EFCoreRepositoryBase<TEntity, TKey> : Repository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  protected override Data.IDataContext Context => ContextCore;

  private protected abstract IDataContext ContextCore { get; }
}

public class EFCoreRepository<TEntity, TKey> : EFCoreRepositoryBase<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public EFCoreRepository(IDataContext context)
  {
    Context = context;
  }

  protected new IDataContext Context { get; }

  private protected override IDataContext ContextCore => Context;

  protected DbContext DbContext => Context.DbContext;

  protected DbSet<TEntity> Entities => Context.DbContext.Set<TEntity>();
}
