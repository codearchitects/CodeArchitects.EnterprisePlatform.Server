using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures.Model;
using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features;

public partial class FeatureTests : IAsyncLifetime
{
  [Fact]
  public void Find_ShouldReturnEntity_WhenExistsAndIsNotSoftDeleted()
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();
    _dbContext.Seed(entity, false);

    // Act
    SoftDeleteEntity? fromDb = _dbContext
      .Set<SoftDeleteEntity>()
      .SingleOrDefault(e => e.Id == entity.Id);

    // Assert
    fromDb.Should().NotBeNull();
    fromDb!.Id.Should().Be(entity.Id);
  }

  [Fact]
  public void Find_ShouldReturnNull_WhenExistsButIsSoftDeleted()
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();
    _dbContext.Seed(entity, true);

    // Act
    SoftDeleteEntity? fromDb = _dbContext
      .Set<SoftDeleteEntity>()
      .SingleOrDefault(e => e.Id == entity.Id);

    // Assert
    fromDb.Should().BeNull();
  }

  [Fact]
  public void Update_ShouldUpdateEntity_WhenEntityIsNotSoftDeleted()
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();
    _dbContext.Seed(entity, false);

    entity.Name = "New entity name";

    // Act
    _dbContext.Update(entity);
    _dbContext.SaveChanges();

    // Assert
    SoftDeleteEntity? fromDb = _dbContext.Find<SoftDeleteEntity>(entity.Id);

    fromDb.Should().NotBeNull();
    fromDb!.Name.Should().Be(entity.Name);
  }

  [Fact]
  public void Update_ShouldThrow_WhenEntityIsSoftDeleted()
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();
    _dbContext.Seed(entity, true);

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
  public void Remove_ShouldSetSoftDeleteFlagToTrue_WhenEntityIsNotAlreadySoftDeleted()
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();
    _dbContext.Seed(entity, false);

    // Act
    _dbContext.Remove(entity);
    _dbContext.SaveChanges();

    // Assert
    SoftDeleteEntity? fromDb = _dbContext.Find<SoftDeleteEntity>(entity.Id);

    fromDb.Should().NotBeNull();
    _dbContext.Entry(fromDb!).Property(SoftDeleteEntity.SoftDeletePropertyName).OriginalValue.Should().Be(true);
  }

  [Fact]
  public void Remove_ShouldThrow_WhenEntityIsAlreadySoftDeleted()
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();
    _dbContext.Seed(entity, true);

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
