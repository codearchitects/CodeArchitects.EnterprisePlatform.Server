using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class Repository<TDbContext, TEntity, TKey> : Repository<TEntity, TKey>
  where TDbContext : DbContext
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public Repository(IDataContext<TDbContext> context)
    : base(context)
  {
    DbContext = context.DbContext;
  }

  protected new TDbContext DbContext { get; }
}
