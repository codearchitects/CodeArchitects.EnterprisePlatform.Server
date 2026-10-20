using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Bson;
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
  public async Task Seeding_ShouldApplyOnce_WhenTwoInstancesSeedConcurrently()
  {
    // Arrange
    // Instance A has found the collection empty and written its seed, but not committed yet.
    // Its keys differ from those of instance B, as they do when each instance generates them.
    Customer[] seedA = [Customer.One(), Customer.One()];
    Customer[] seedB = [Customer.One(), Customer.One(), Customer.One()];

    using TestScope scope = _fixture.CreateScope();
    IMongoCollection<Customer> customers = _fixture.GetCollection<Customer>();
    IMongoCollection<BsonDocument> markers = scope.Get<IMongoDatabase>()
      .GetCollection<BsonDocument>(Seeder.MarkerCollectionName);

    // The collections exist up front: their implicit creation is not what this test is about.
    await scope.Get<IMongoDatabase>().CreateCollectionAsync(customers.CollectionNamespace.CollectionName);
    await scope.Get<IMongoDatabase>().CreateCollectionAsync(Seeder.MarkerCollectionName);

    using IClientSessionHandle instanceA = await scope.Get<IMongoClient>().StartSessionAsync();
    instanceA.StartTransaction();
    await markers.UpdateOneAsync(
      instanceA,
      Builders<BsonDocument>.Filter.Eq("_id", customers.CollectionNamespace.CollectionName),
      Builders<BsonDocument>.Update.CurrentDate("seededAt"),
      new UpdateOptions { IsUpsert = true });
    await customers.InsertManyAsync(instanceA, seedA);

    // Act
    // Instance B sees the collection empty too, and must wait for A instead of seeding as well.
    Task instanceB = _fixture.SeedAsync(seedB);
    await Task.Delay(TimeSpan.FromSeconds(1));
    bool completedBeforeCommit = instanceB.IsCompleted;

    await instanceA.CommitTransactionAsync();
    await instanceB;

    // Assert
    completedBeforeCommit.Should().BeFalse();
    List<Customer> seeded = await customers.Find(FilterDefinition<Customer>.Empty).ToListAsync();
    seeded.Should().BeEquivalentTo(seedA);
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
