using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public interface IEFCoreContext<TDbContext> : IEFCoreContext
  where TDbContext : DbContext
{
  new TDbContext DbContext { get; }
}
