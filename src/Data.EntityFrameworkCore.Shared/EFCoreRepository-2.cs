using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public abstract class EFCoreRepositoryBase<TEntity, TKey> : Repository<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  protected override IDataContext Context => ContextCore;

  private protected abstract IEFCoreContext ContextCore { get; }
}

public class EFCoreRepository<TEntity, TKey> : EFCoreRepositoryBase<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public EFCoreRepository(IEFCoreContext context)
  {
    Context = context;
  }

  protected new IEFCoreContext Context { get; }

  private protected override IEFCoreContext ContextCore => Context;

  protected DbContext DbContext => Context.DbContext;

  protected DbSet<TEntity> Entities => Context.DbContext.Set<TEntity>(EntityName);
}
