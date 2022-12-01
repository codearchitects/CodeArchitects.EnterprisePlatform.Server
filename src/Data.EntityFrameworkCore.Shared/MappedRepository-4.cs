using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

internal abstract class MappedRepository<TDbContext, TTable, TEntity, TKey> : MappedRepository<TTable, TEntity, TKey>
  where TDbContext : DbContext
  where TTable : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public MappedRepository(IDataContext<TDbContext> context)
    : base(context)
  {
    DbContext = context.DbContext;
  }

  protected new TDbContext DbContext { get; }
}
