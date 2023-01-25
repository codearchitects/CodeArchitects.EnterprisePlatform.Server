using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

internal class StateManager<TDbContext> : StateManager, IStateManager<TDbContext>
  where TDbContext : DbContext
{
  public StateManager(TDbContext dbContext)
  {
    DbContext = dbContext;
  }

  public TDbContext DbContext { get; }

  protected override Task SaveCoreAsync(CancellationToken cancellationToken)
  {
    return DbContext.SaveChangesAsync(cancellationToken);
  }
}
