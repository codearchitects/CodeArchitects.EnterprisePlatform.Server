using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

[Collection(TestCollection.Name)]
public class UnitOfWorkTests : TestBase
{
  private readonly Customer[] _seed;

  public UnitOfWorkTests(TestFixture fixture) : base(fixture)
  {
    _seed = new Customer[]
    {
      Customer.One(),
      Customer.One()
    };
  }

  [Fact]
  public async Task InsertAsync_ShouldInsertEntity_WhenEntityNotExists()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>();
    IUnitOfWorkManager uowManager = _fixture.GetUnitOfWorkManager();
    IMongoCollection <Customer> collection = _fixture.GetCollection<Customer>();

    // Act
    await using (IUnitOfWork uow = uowManager.Begin(autoSave: true))
    {
      await sut.InsertAsync(_seed[0]);
      await sut.InsertAsync(_seed[1]);
    }

    Customer? customer1 = await collection.Find(x => x.Id == _seed[0].Id).FirstOrDefaultAsync();
    Customer? customer2 = await collection.Find(x => x.Id == _seed[1].Id).FirstOrDefaultAsync();

    // Assert
    customer1.Should().NotBeNull();
    customer1.Should().BeEquivalentTo(_seed[0]);
    customer2.Should().NotBeNull();
    customer2.Should().BeEquivalentTo(_seed[1]);
  }

  [Fact]
  public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityExists()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>(_seed);
    IUnitOfWorkManager uowManager = _fixture.GetUnitOfWorkManager();
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();
    Customer expected1 = _seed[0];
    Customer expected2 = _seed[1];

    // Act
    expected1.Name = "ModifiedName1";
    expected2.Name = "ModifiedName2";

    await using (IUnitOfWork uow = uowManager.Begin(autoSave: true))
    {
      await sut.UpdateAsync(expected1);
      await sut.UpdateAsync(expected2);
    }

    Customer? customer1 = await collection.Find(x => x.Id == expected1.Id).FirstOrDefaultAsync();
    Customer? customer2 = await collection.Find(x => x.Id == expected2.Id).FirstOrDefaultAsync();

    // Assert
    customer1.Should().NotBeNull();
    customer1.Should().BeEquivalentTo(expected1);
    customer2.Should().NotBeNull();
    customer2.Should().BeEquivalentTo(expected2);
  }

  [Fact]
  public async Task RemoveAsync_ShouldRemoveEntity_WhenEntityExists()
  {
    // Arrange
    MongoDBRepository<Customer, Guid> sut = _fixture.CreateRepository<Customer, Guid>(_seed);
    IUnitOfWorkManager uowManager = _fixture.GetUnitOfWorkManager();
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();

    // Act
    await using (IUnitOfWork uow = uowManager.Begin(autoSave: true))
    {
      await sut.RemoveAsync(_seed[0]);
      await sut.RemoveAsync(_seed[1]);
    }

    Customer? customer1 = await collection.Find(x => x.Id == _seed[0].Id).FirstOrDefaultAsync();
    Customer? customer2 = await collection.Find(x => x.Id == _seed[1].Id).FirstOrDefaultAsync();

    // Assert
    customer1.Should().BeNull();
    customer2.Should().BeNull();
  }
}
