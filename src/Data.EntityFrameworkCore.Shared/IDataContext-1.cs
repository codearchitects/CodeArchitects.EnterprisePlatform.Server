using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public interface IDataContext<TDbContext> : IDataContext
  where TDbContext : DbContext
{
  new TDbContext DbContext { get; }
}
