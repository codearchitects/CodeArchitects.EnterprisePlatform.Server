using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public abstract class EFCoreIntegrationTest<TContext>
  where TContext : DbContext
{
  public EFCoreIntegrationTest(ITestOutputHelper output)
  {
    Output = output;

    DbContextOptions<TContext> options = new DbContextOptionsBuilder<TContext>()
      .UseSqlServer($"Server=localhost,1433;Database={GetType().Name};User Id=sa;Password=Password1")
      .LogTo(output.WriteLine)
      .EnableSensitiveDataLogging()
      .Options;
    Context = CreateContext(options);
    Context.Database.EnsureDeleted();
    Context.Database.EnsureCreated();
  }

  protected ITestOutputHelper Output { get; }
  protected TContext Context { get; }

  protected abstract TContext CreateContext(DbContextOptions<TContext> options);
}
