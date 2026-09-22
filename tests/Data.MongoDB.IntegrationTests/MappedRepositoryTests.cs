using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Driver;
using System.Data;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// The mapped repository used to be unusable: <c>Collection</c> asked for the collection of the
/// domain entity, which is not registered in the model, so every access threw.
/// </summary>
[Xunit.Collection(TestCollection.Name)]
public class MappedRepositoryTests(TestFixture fixture) : TestBase(fixture)
{
  [Fact]
  public void Collection_ShouldResolveTheDocumentCollection()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    OrderRepository repository = new(scope.Context);

    // Act
    Action act = () => _ = repository.ExposedDocuments;

    // Assert
    act.Should().NotThrow();
  }

  [Fact]
  public async Task InsertAsync_ShouldPersistTheDocumentShape()
  {
    // Arrange
    Order order = Order.One();
    using TestScope scope = _fixture.CreateScope();
    OrderRepository repository = new(scope.Context);

    // Act
    await repository.InsertAsync(order);

    // Assert
    // What is stored is the document, with its own field names.
    OrderDocument? document = await _fixture.GetCollection<OrderDocument>()
      .Find(x => x.Id == order.Id).FirstOrDefaultAsync();

    document.Should().NotBeNull();
    document!.Code.Should().Be(order.Reference);
    document.Total.Should().Be(order.Amount);
  }

  [Fact]
  public async Task FindAsync_ShouldReturnTheDomainShape()
  {
    // Arrange
    Order expected = Order.One();
    using TestScope scope = _fixture.CreateScope();
    OrderRepository repository = new(scope.Context);
    await repository.InsertAsync(expected);

    // Act
    Order? order = await repository.FindAsync(expected.Id);

    // Assert
    order.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task UpdateAsync_ShouldRoundTripThroughTheMapping()
  {
    // Arrange
    Order order = Order.One();
    using TestScope scope = _fixture.CreateScope();
    OrderRepository repository = new(scope.Context);
    await repository.InsertAsync(order);

    // Act
    order.Reference = "ORD-0001";
    await repository.UpdateAsync(order);

    // Assert
    Order? updated = await repository.FindAsync(order.Id);
    updated!.Reference.Should().Be("ORD-0001");
  }

  [Fact]
  public async Task RemoveAsync_ShouldDeleteByKey()
  {
    // Arrange
    Order order = Order.One();
    using TestScope scope = _fixture.CreateScope();
    OrderRepository repository = new(scope.Context);
    await repository.InsertAsync(order);

    // Act
    await repository.RemoveAsync(order.Id);

    // Assert
    (await repository.FindAsync(order.Id)).Should().BeNull();
  }

  [Fact]
  public async Task UpdateAsync_ShouldThrow_WhenTheDocumentDoesNotExist()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    OrderRepository repository = new(scope.Context);

    // Act
    Func<Task> act = () => repository.UpdateAsync(Order.One());

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();
  }
}
