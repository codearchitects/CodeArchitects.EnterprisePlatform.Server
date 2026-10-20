using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// An entity whose key follows the &lt;TypeName&gt;Id convention, which the driver does not map to
/// _id on its own: every operation by key must still find the document.
/// </summary>
[Xunit.Collection(TestCollection.Name)]
public class KeyConventionTests(TestFixture fixture) : TestBase(fixture)
{
  private Task<BsonDocument?> Stored(Guid id)
  {
    IMongoCollection<Supplier> collection = _fixture.GetCollection<Supplier>();

    return collection.Database
      .GetCollection<BsonDocument>(collection.CollectionNamespace.CollectionName)
      .Find(Builders<BsonDocument>.Filter.Eq("_id", new BsonBinaryData(id, GuidRepresentation.Standard)))
      .FirstOrDefaultAsync()!;
  }

  [Fact]
  public async Task Insert_ShouldStoreTheKeyInTheIdElement()
  {
    // Arrange
    Supplier supplier = Supplier.One();
    using TestScope scope = _fixture.CreateScope();

    // Act
    await scope.Repository<Supplier, Guid>().InsertAsync(supplier);

    // Assert
    BsonDocument? stored = await Stored(supplier.SupplierId);
    stored.Should().NotBeNull();
    stored!.Names.Should().NotContain(nameof(Supplier.SupplierId));
  }

  [Fact]
  public async Task Find_ShouldReturnTheEntity()
  {
    // Arrange
    Supplier expected = Supplier.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Supplier? supplier = await scope.Repository<Supplier, Guid>().FindAsync(expected.SupplierId);

    // Assert
    supplier.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task Update_ShouldReplaceTheDocument()
  {
    // Arrange
    Supplier expected = Supplier.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Supplier, Guid> repository = scope.Repository<Supplier, Guid>();

    // Act
    expected.Name = "Renamed";
    await repository.UpdateAsync(expected);

    // Assert
    (await repository.FindAsync(expected.SupplierId)).Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task Remove_ShouldDeleteTheDocument()
  {
    // Arrange
    Supplier existing = Supplier.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    await scope.Repository<Supplier, Guid>().RemoveAsync(existing.SupplierId);

    // Assert
    (await Stored(existing.SupplierId)).Should().BeNull();
  }
}
