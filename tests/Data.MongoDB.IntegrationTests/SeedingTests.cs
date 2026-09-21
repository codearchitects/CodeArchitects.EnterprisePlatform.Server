using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

[Xunit.Collection(TestCollection.Name)]
public class SeedingTests(TestFixture fixture) : TestBase(fixture)
{
  [Fact]
  public async Task Seeding_ShouldPopulateAnEmptyCollection()
  {
    // Arrange
    Customer[] seed = [Customer.One(), Customer.One()];

    // Act
    await _fixture.SeedAsync(seed);

    // Assert
    List<Customer> customers = await _fixture.GetCollection<Customer>()
      .Find(FilterDefinition<Customer>.Empty).ToListAsync();
    customers.Should().BeEquivalentTo(seed);
  }

  [Fact]
  public async Task Seeding_ShouldBeIdempotent()
  {
    // Arrange
    Customer[] seed = [Customer.One(), Customer.One()];
    await _fixture.SeedAsync(seed);

    // Act
    await _fixture.SeedAsync([Customer.One(), Customer.One(), Customer.One()]);

    // Assert
    // Idempotency is per collection: a non-empty collection is left untouched, whatever the
    // second seed contains.
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();
    (await collection.CountDocumentsAsync(FilterDefinition<Customer>.Empty)).Should().Be(2);
  }

  [Fact]
  public async Task Seeding_ShouldApplyNothing_WhenOneCollectionFails()
  {
    // Arrange
    Customer duplicate = Customer.One();

    // Act
    Func<Task> act = () => _fixture.SeedAsync([duplicate, duplicate]);

    // Assert
    await act.Should().ThrowAsync<MongoException>();

    // Every seed is committed as a single unit: a failing one must not leave the database
    // half populated.
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();
    (await collection.CountDocumentsAsync(FilterDefinition<Customer>.Empty)).Should().Be(0);
  }

  [Fact]
  public async Task Seeding_ShouldSpanSeveralCollections_InOneCommit()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();

    // Act
    await _fixture.SeedAsync([Customer.One()]);
    await _fixture.SeedAsync([Cart.One()]);

    // Assert
    (await _fixture.GetCollection<Customer>().CountDocumentsAsync(FilterDefinition<Customer>.Empty))
      .Should().Be(1);
    (await _fixture.GetCollection<Cart>().CountDocumentsAsync(FilterDefinition<Cart>.Empty))
      .Should().Be(1);
  }
}
