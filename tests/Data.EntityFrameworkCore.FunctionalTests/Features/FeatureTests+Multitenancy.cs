using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures.Model;
using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features;

public partial class FeatureTests
{
  [Fact]
  public void Seeding_ShouldBypassMultitenancy_WhenMultitenancyIsEnabled()
  {
    // Arrange
    TenantEntity entity = TenantEntity.One();
    TestDataSeed seed = new(entity);
    Seeder seeder = new(_dbContext);

    // Act
    seeder.Apply(seed);
    TenantEntity? fromDb = _dbContext
      .Set<TenantEntity>()
      .SingleOrDefault(e => e.Id == entity.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(entity.Id);
  }

  [Fact]
  public void Seeding_ShouldNotFail_WhenMultitenancyIsDisabled()
  {
    // Arrange
    using TestDbContext dbContext = new TestDbContext(s_options, false);
    _dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
    TenantEntity entity = TenantEntity.One();
    TestDataSeed seed = new(entity);
    Seeder seeder = new(_dbContext);

    // Act
    seeder.Apply(seed);
    TenantEntity? fromDb = _dbContext
      .Set<TenantEntity>()
      .SingleOrDefault(e => e.Id == entity.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(entity.Id);
    dbContext.Database.EnsureDeleted();
  }

  [Fact]
  public void Find_ShouldReturnEntity_WhenExistsAndBelogsToCurrentTenant()
  {
    // Arrange
    Guid tenantId = Guid.NewGuid();
    TestDbContext.MultitenancyContext.TenantId = tenantId;

    TenantEntity entity = TenantEntity.One();
    _dbContext.Seed(entity);

    // Act
    TenantEntity? fromDb = _dbContext
      .Set<TenantEntity>()
      .SingleOrDefault(e => e.Id == entity.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(entity.Id);
  }

  [Fact]
  public void Find_ShouldReturnNull_WhenExistsButBelongsToAnotherTenant()
  {
    // Arrange
    Guid tenantId = Guid.NewGuid();
    TestDbContext.MultitenancyContext.TenantId = tenantId;

    TenantEntity entity = TenantEntity.One();
    _dbContext.Seed(entity);

    TestDbContext.MultitenancyContext.TenantId = Guid.NewGuid();

    // Act
    TenantEntity? fromDb = _dbContext
      .Set<TenantEntity>()
      .SingleOrDefault(e => e.Id == entity.Id);

    // Assert
    fromDb.Should().BeNull();
  }

  [Fact]
  public void Add_ShouldInsertEntityWithCurrentTenantId()
  {
    // Arrange
    Guid tenantId = Guid.NewGuid();
    TestDbContext.MultitenancyContext.TenantId = tenantId;

    TenantEntity entity = TenantEntity.One();

    // Act
    _dbContext.Add(entity);
    _dbContext.SaveChanges();

    // Assert
    TenantEntity? fromDb = _dbContext.Find<TenantEntity>(entity.Id);

    fromDb.Should().NotBeNull();
    _dbContext.Entry(fromDb!).Property(TenantEntity.TenantIdPropertyName).CurrentValue.Should().Be(tenantId);
  }

  [Fact]
  public void Update_ShouldUpdateEntity_WhenEntityBelongsToCurrentTenant()
  {
    // Arrange
    Guid tenantId = Guid.NewGuid();
    TestDbContext.MultitenancyContext.TenantId = tenantId;

    TenantEntity entity = TenantEntity.One();
    _dbContext.Seed(entity);

    entity.Name = "New entity name";

    // Act
    _dbContext.Update(entity);
    _dbContext.SaveChanges();

    // Assert
    TenantEntity? fromDb = _dbContext.Find<TenantEntity>(entity.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(entity.Name);
  }

  [Fact]
  public void Update_ShouldThrow_WhenEntityBelongsToAnotherTenant()
  {
    // Arrange
    TestDbContext.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();
    _dbContext.Seed(entity);

    TestDbContext.MultitenancyContext.TenantId = Guid.NewGuid();

    // Act
    Action act = () =>
    {
      _dbContext.Update(entity);
      _dbContext.SaveChanges();
    };

    // Assert
    act.Should().Throw<DbUpdateConcurrencyException>();
  }

  [Fact]
  public void Remove_ShouldRemoveEntity_WhenEntityBelongsToCurrentTenant()
  {
    // Arrange
    TestDbContext.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();
    _dbContext.Seed(entity);

    // Act
    _dbContext.Remove(entity);
    _dbContext.SaveChanges();

    // Assert
    TenantEntity? fromDb = _dbContext.Find<TenantEntity>(entity.Id);

    fromDb.Should().BeNull();
  }

  [Fact]
  public void Remove_ShouldThrow_WhenEntityBelongsToAnotherTenant()
  {
    // Arrange
    TestDbContext.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();
    _dbContext.Seed(entity);

    TestDbContext.MultitenancyContext.TenantId = Guid.NewGuid();

    // Act
    Action act = () =>
    {
      _dbContext.Remove(entity);
      _dbContext.SaveChanges();
    };

    // Assert
    act.Should().Throw<DbUpdateConcurrencyException>();
  }
}
