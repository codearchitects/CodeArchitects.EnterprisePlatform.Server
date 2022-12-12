using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class EFCoreRepository<TDbContext, TEntity, TKey> : EFCoreRepository<TEntity, TKey>
  where TDbContext : DbContext
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public EFCoreRepository(IEFCoreContext<TDbContext> context)
    : base(context)
  {
  }

  protected new IEFCoreContext<TDbContext> Context => (IEFCoreContext<TDbContext>)base.Context;

  protected new TDbContext DbContext => Context.DbContext;
}
