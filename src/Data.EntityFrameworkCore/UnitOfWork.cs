using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class UnitOfWork<TContext> : IUnitOfWork
  where TContext : DbContext
{
  public UnitOfWork(TContext context)
  {
    Context = context;
  }

  protected TContext Context { get; }

  public virtual void Save()
  {
    Context.SaveChanges();
  }

  public virtual async Task SaveAsync(CancellationToken cancellationToken = default)
  {
    await Context.SaveChangesAsync(cancellationToken);
  }
}
