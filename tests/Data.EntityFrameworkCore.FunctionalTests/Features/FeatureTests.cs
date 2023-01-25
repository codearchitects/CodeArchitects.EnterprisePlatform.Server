using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features;

[Collection("SQLite")]
public partial class FeatureTests
{
  private readonly TestDbContext _dbContext;

  public FeatureTests()
  {
    _dbContext = new(new DbContextOptionsBuilder()
      .UseSqlite("DataSource=file::memory:?cache=shared")
      .Options);
  }

  public Task InitializeAsync()
  {
    _dbContext.Database.EnsureCreated();
    return Task.CompletedTask;
  }

  public Task DisposeAsync()
  {
    _dbContext.Database.EnsureDeleted();
    _dbContext.Dispose();
    return Task.CompletedTask;
  }
}
