using CodeArchitects.Platform.Data.Fixtures;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data;

[Collection(TestCollection.Name)]
public class RemoveTests : TestBase
{
  public RemoveTests(TestFixture fixture, ITestOutputHelper output)
    : base(fixture, output)
  {
  }

  [Theory, RepositoryDependenciesData]
  public async Task RemoveAsync_ShouldRemoveEntity(RepositoryDependencies dependencies)
  {
    // Arrange
    Product product = Product.One();

    using var dbFixture = _fixture.CreateDbFixture(dependencies);
    dbFixture.Seed(seeder => seeder.Seed(product));
    using var scope = _fixture.CreateScope(dependencies);
    var sut = scope.CreateRepository<Product, Guid>();

    // Act
    await sut.RemoveAsync(product.Id);

    // Assert
    scope.DbContext.Set<Product>().FirstOrDefault(p => p.Id == product.Id).Should().BeNull();
  }
}
