using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class EFCoreRepository<TDbContext, TEntity, TKey> : EFCoreRepository<TEntity, TKey>
  where TDbContext : DbContext
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public EFCoreRepository(IDataContext<TDbContext> context)
    : base(context)
  {
  }

  protected new IDataContext<TDbContext> Context => (IDataContext<TDbContext>)base.Context;

  protected new TDbContext DbContext => Context.DbContext;
}
