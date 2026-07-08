using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Driver;
using System.Data;

namespace CodeArchitects.Platform.Data.MongoDB;

[Collection(TestCollection.Name)]
public class RepositoryTests : TestBase
{
  private readonly Customer[] _seed;

  public RepositoryTests(TestFixture fixture) : base(fixture)
  {
    _seed = new Customer[]
    {
      Customer.One()
    };
  }

  [Fact]
  public async Task FindAsync_ShouldReturnEntity_WhenEntityExists()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>(_seed);

    // Act
    Customer? customer = await sut.FindAsync(_seed[0].Id);

    // Assert
    customer.Should().NotBeNull();
    customer.Should().BeEquivalentTo(_seed[0]);
  }

  [Fact]
  public async Task FindAsync_ShouldReturnNull_WhenEntityDoesNotExist()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>();

    // Act
    Customer? customer = await sut.FindAsync(_seed[0].Id);

    // Assert
    customer.Should().BeNull();
  }

  [Fact]
  public async Task InsertAsync_ShouldInsertEntity_WhenEntityDoesNotExist()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>();
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();

    // Act
    await sut.InsertAsync(_seed[0]);
    Customer? customer = await collection.Find(x => x.Id == _seed[0].Id).FirstOrDefaultAsync();

    // Assert
    customer.Should().NotBeNull();
    customer.Should().BeEquivalentTo(_seed[0]);
  }

  [Fact]
  public async Task InsertAsync_ShouldThrow_WhenEntityAlreadyExists()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>(_seed);

    // Act
    Func<Task> act = () => sut.InsertAsync(_seed[0]);

    // Assert
    await act.Should().ThrowAsync<MongoWriteException>();
  }

  [Fact]
  public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityExists()
  {
    // Arrange
    Customer expected = _seed[0];
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>(_seed);
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();

    // Act
    expected.Name = "ModifiedName";
    await sut.UpdateAsync(expected);
    Customer? customer = await collection.Find(x => x.Id == expected.Id).FirstOrDefaultAsync();

    // Assert
    customer.Should().NotBeNull();
    customer.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task UpdateAsync_ShouldThrow_WhenEntityDoesNotExist()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>();

    // Act
    Func<Task> act = () => sut.UpdateAsync(_seed[0]);

    // Assert
    await act.Should().ThrowAsync<Exception>("Object not updated.");
  }

  [Fact]
  public async Task UpsertAsync_ShouldUpdateEntity_WhenEntityExists()
  {
    // Arrange
    Customer expected = _seed[0];
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>(_seed);
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();

    // Act
    expected.Name = "ModifiedName";
    await sut.UpsertAsync(expected);
    Customer? customer = await collection.Find(x => x.Id == expected.Id).FirstOrDefaultAsync();

    // Assert
    customer.Should().NotBeNull();
    customer.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task UpsertAsync_ShouldInsertEntity_WhenEntityDoesNotExist()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>();
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();

    // Act
    await sut.UpsertAsync(_seed[0]);
    Customer? customer = await collection.Find(x => x.Id == _seed[0].Id).FirstOrDefaultAsync();

    // Assert
    customer.Should().NotBeNull();
    customer.Should().BeEquivalentTo(_seed[0]);
  }

  [Fact]
  public async Task RemoveAsync_ShouldRemoveEntity_WhenEntityExists()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>(_seed);
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();

    // Act
    await sut.RemoveAsync(_seed[0]);
    Customer? customer = await collection.Find(x => x.Id == _seed[0].Id).FirstOrDefaultAsync();

    // Assert
    customer.Should().BeNull();
  }

  [Fact]
  public async Task RemoveAsync_ShouldThrow_WhenEntityNotExists()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>();

    // Act
    Func<Task> act = () => sut.RemoveAsync(_seed[0]);

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();
  }

  [Fact]
  public async Task RemoveAsync_ShouldRemoveEntity_WhenEntityKeyExists()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>(_seed);
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();

    // Act
    await sut.RemoveAsync(_seed[0].Id);
    Customer? customer = await collection.Find(x => x.Id == _seed[0].Id).FirstOrDefaultAsync();

    // Assert
    customer.Should().BeNull();
  }

  [Fact]
  public async Task RemoveAsync_ShouldThrow_WhenEntityKeyNotExists()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>();

    // Act
    Func<Task> act = () => sut.RemoveAsync(_seed[0].Id);

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();
  }
}
