using CodeArchitects.Platform.Data.Fixtures;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data;

[Collection(TestCollection.Name)]
public class RemoveAsyncTests : TestBase
{
  public RemoveAsyncTests(TestFixture fixture, ITestOutputHelper output)
    : base(fixture, output)
  {
  }

  [Theory, RepositoryDependenciesData]
  public async Task RemoveAsync_ShouldRemoveEntity(RepositoryDependencies dependencies)
  {
    // Arrange
    Product product = Product.One();

    var sut = _fixture.CreateRepository<Product, Guid>(dependencies, new[] { product });

    // Act
    await sut.RemoveAsync(product.Id);

    // Assert
    _fixture.DbContext.Set<Product>().FirstOrDefault(p => p.Id == product.Id).Should().BeNull();
  }

  [MultitenancyTest]
  [Theory, RepositoryDependenciesData]
  public async Task RemoveAsync_ShouldRemoveEntity_WhenEntityBelongsToCurrentTenant(RepositoryDependencies dependencies)
  {
    // Arrange
    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();

    var sut = _fixture.CreateRepository<TenantEntity, Guid>(dependencies, new[] { entity });

    // Act
    await sut.RemoveAsync(entity.Id);

    // Assert
    _fixture.DbContext.Set<TenantEntity>().FirstOrDefault(e => e.Id == entity.Id).Should().BeNull();
  }

  [MultitenancyTest]
  [Theory, RepositoryDependenciesData]
  public async Task RemoveAsync_ShouldThrow_WhenEntityBelongsToAnotherTenant(RepositoryDependencies dependencies)
  {
    // Arrange
    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    TenantEntity entity = TenantEntity.One();

    var sut = _fixture.CreateRepository<TenantEntity, Guid>(dependencies, new[] { entity });

    _fixture.MultitenancyContext.TenantId = Guid.NewGuid();

    // Act
    Func<Task> act = () => sut.RemoveAsync(entity.Id);

    // Assert
    await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
  }

  [SoftDeleteTest]
  [Theory, RepositoryDependenciesData]
  public async Task RemoveAsync_ShouldSetSoftDeleteFlagToTrue_WhenEntityIsNotAlreadySoftDeleted(RepositoryDependencies dependencies)
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();

    var sut = _fixture.CreateRepository<SoftDeleteEntity, Guid>(dependencies, new[] { entity });

    // Act
    await sut.RemoveAsync(entity.Id);

    // Assert
    _fixture.SoftDeleteContext.ShouldFilter = false;
    SoftDeleteEntity? fromDb = _fixture.DbContext.Set<SoftDeleteEntity>().FirstOrDefault(e => e.Id == entity.Id);
    fromDb.Should().NotBeNull();
    _fixture.DbContext.Entry(fromDb!).Property(SoftDeleteEntity.SoftDeletePropertyName).OriginalValue.Should().Be(true);
  }

  [SoftDeleteTest]
  [Theory, RepositoryDependenciesData]
  public async Task RemoveAsync_ShouldThrow_WhenEntityIsAlreadySoftDeleted(RepositoryDependencies dependencies)
  {
    // Arrange
    SoftDeleteEntity entity = SoftDeleteEntity.One();

    var sut = _fixture.CreateRepository<SoftDeleteEntity, Guid>(dependencies, new[] { entity });

    _fixture.SoftDelete(entity);

    // Act
    Func<Task> act = () => sut.RemoveAsync(entity.Id);

    // Assert
    await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
  }
}
