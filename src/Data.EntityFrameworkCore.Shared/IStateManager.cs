using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

internal interface IStateManager<TDbContext> : IStateManager
  where TDbContext : DbContext
{
  TDbContext DbContext { get; }
}
