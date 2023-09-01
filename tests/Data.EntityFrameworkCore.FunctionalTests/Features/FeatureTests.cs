using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features;

[Collection("SQLite")]
public sealed partial class FeatureTests : IDisposable
{
  private static readonly DbContextOptions s_options = new DbContextOptionsBuilder()
    .UseSqlite("DataSource=file::memory:?cache=shared")
    .Options;

  private readonly TestDbContext _dbContext;
  private readonly ITestOutputHelper _output;

  public FeatureTests(ITestOutputHelper output)
  {
    _dbContext = new(s_options);
    _output = output;
    _dbContext.Database.EnsureCreated();
  }

  public void Dispose()
  {
    _dbContext.Database.ExecuteSqlRaw("DELETE FROM TenantEntity");
    _dbContext.Database.ExecuteSqlRaw("DELETE FROM SoftDeleteEntity");
    _dbContext.Database.EnsureDeleted();
    _dbContext.Dispose();
  }
}
