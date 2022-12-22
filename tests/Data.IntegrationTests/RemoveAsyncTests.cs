using CodeArchitects.Platform.Data.Fixtures;
using CodeArchitects.Platform.Data.Fixtures.Model;
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
}
