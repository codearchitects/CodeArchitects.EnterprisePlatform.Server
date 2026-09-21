using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Driver;
using System.Data;

namespace CodeArchitects.Platform.Data.MongoDB;

[Xunit.Collection(TestCollection.Name)]
public class RepositoryTests(TestFixture fixture) : TestBase(fixture)
{
  private static MongoDBRepository<Customer, Guid> Repository(TestScope scope) => scope.Repository<Customer, Guid>();

  #region Find

  [Fact]
  public async Task FindAsync_ShouldReturnEntity_WhenEntityExists()
  {
    // Arrange
    Customer expected = Customer.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Customer? customer = await Repository(scope).FindAsync(expected.Id);

    // Assert
    customer.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task FindAsync_ShouldReturnNull_WhenEntityDoesNotExist()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();

    // Act
    Customer? customer = await Repository(scope).FindAsync(Guid.NewGuid());

    // Assert
    customer.Should().BeNull();
  }

  [Fact]
  public async Task FindAsync_ShouldThrow_WhenIncludeIsRequested()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Repository(scope).FindAsync(Guid.NewGuid(), _ => { });

    // Assert
    // No silent degradation: Include is not supported yet and says so.
    await act.Should().ThrowAsync<NotSupportedException>();
  }

  #endregion

  #region Insert

  [Fact]
  public async Task InsertAsync_ShouldInsertEntity_WhenEntityDoesNotExist()
  {
    // Arrange
    Customer expected = Customer.One();
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Repository(scope).InsertAsync(expected);

    // Assert
    Customer? customer = await _fixture.GetCollection<Customer>()
      .Find(x => x.Id == expected.Id).FirstOrDefaultAsync();
    customer.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task InsertAsync_ShouldThrow_WhenEntityAlreadyExists()
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Repository(scope).InsertAsync(existing);

    // Assert
    await act.Should().ThrowAsync<MongoWriteException>();
  }

  [Fact]
  public async Task InsertManyAsync_ShouldInsertEveryEntity()
  {
    // Arrange
    Customer[] expected = [Customer.One(), Customer.One(), Customer.One()];
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Repository(scope).InsertManyAsync(expected);

    // Assert
    List<Customer> customers = await _fixture.GetCollection<Customer>()
      .Find(FilterDefinition<Customer>.Empty).ToListAsync();
    customers.Should().BeEquivalentTo(expected);
  }

  #endregion

  #region Update

  [Fact]
  public async Task UpdateAsync_ShouldUpdateEntity_WhenEntityExists()
  {
    // Arrange
    Customer expected = Customer.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    expected.Name = "ModifiedName";
    await Repository(scope).UpdateAsync(expected);

    // Assert
    Customer? customer = await _fixture.GetCollection<Customer>()
      .Find(x => x.Id == expected.Id).FirstOrDefaultAsync();
    customer.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task UpdateAsync_ShouldNotThrow_WhenNothingChanged()
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Repository(scope).UpdateAsync(existing);

    // Assert
    // A replace that matches but rewrites nothing reports ModifiedCount == 0. The success
    // criterion is MatchedCount, otherwise an idempotent update would look like a conflict.
    await act.Should().NotThrowAsync();
  }

  [Fact]
  public async Task UpdateAsync_ShouldThrow_WhenEntityDoesNotExist()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Repository(scope).UpdateAsync(Customer.One());

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();
  }

  [Fact]
  public async Task UpdateManyAsync_ShouldUpdateEveryEntity()
  {
    // Arrange
    Customer[] seed = [Customer.One(), Customer.One()];
    await _fixture.SeedAsync(seed);
    using TestScope scope = _fixture.CreateScope();

    // Act
    seed[0].Name = "First";
    seed[1].Name = "Second";
    await Repository(scope).UpdateManyAsync(seed);

    // Assert
    List<Customer> customers = await _fixture.GetCollection<Customer>()
      .Find(FilterDefinition<Customer>.Empty).ToListAsync();
    customers.Select(customer => customer.Name).Should().BeEquivalentTo(["First", "Second"]);
  }

  [Fact]
  public async Task UpdateManyAsync_ShouldThrowAndApplyNothing_WhenOneEntityDoesNotExist()
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    existing.Name = "Modified";
    Func<Task> act = () => Repository(scope).UpdateManyAsync([existing, Customer.One()]);

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();

    Customer? customer = await _fixture.GetCollection<Customer>()
      .Find(x => x.Id == existing.Id).FirstOrDefaultAsync();
    customer!.Name.Should().NotBe("Modified", "the bulk write runs inside a transaction");
  }

  #endregion

  #region Upsert

  [Fact]
  public async Task UpsertAsync_ShouldUpdateEntity_WhenEntityExists()
  {
    // Arrange
    Customer expected = Customer.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    expected.Name = "ModifiedName";
    await Repository(scope).UpsertAsync(expected);

    // Assert
    Customer? customer = await _fixture.GetCollection<Customer>()
      .Find(x => x.Id == expected.Id).FirstOrDefaultAsync();
    customer.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task UpsertAsync_ShouldInsertEntity_WhenEntityDoesNotExist()
  {
    // Arrange
    Customer expected = Customer.One();
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Repository(scope).UpsertAsync(expected);

    // Assert
    Customer? customer = await _fixture.GetCollection<Customer>()
      .Find(x => x.Id == expected.Id).FirstOrDefaultAsync();
    customer.Should().BeEquivalentTo(expected);
  }

  #endregion

  #region Remove

  [Fact]
  public async Task RemoveAsync_ShouldRemoveEntity_WhenEntityExists()
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Repository(scope).RemoveAsync(existing);

    // Assert
    Customer? customer = await _fixture.GetCollection<Customer>()
      .Find(x => x.Id == existing.Id).FirstOrDefaultAsync();
    customer.Should().BeNull();
  }

  [Fact]
  public async Task RemoveAsync_ShouldRemoveEntity_WhenEntityKeyExists()
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Repository(scope).RemoveAsync(existing.Id);

    // Assert
    Customer? customer = await _fixture.GetCollection<Customer>()
      .Find(x => x.Id == existing.Id).FirstOrDefaultAsync();
    customer.Should().BeNull();
  }

  [Fact]
  public async Task RemoveAsync_ShouldThrow_WhenEntityDoesNotExist()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Repository(scope).RemoveAsync(Guid.NewGuid());

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();
  }

  #endregion

  #region Aggregates

  [Fact]
  public async Task Repository_ShouldRoundTripAnAggregate_WithEmbeddedDocuments()
  {
    // Arrange
    Cart expected = Cart.One();
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Cart, Guid> repository = scope.Repository<Cart, Guid>();

    // Act
    await repository.InsertAsync(expected);
    Cart? cart = await repository.FindAsync(expected.Id);

    // Assert
    // Intra-aggregate children live in the same document: no Include, and the whole aggregate
    // is written in a single atomic operation.
    cart.Should().BeEquivalentTo(expected);
    cart!.Items.Should().HaveCount(expected.Items.Count);
  }

  [Fact]
  public async Task Repository_ShouldReplaceTheWholeAggregate_OnUpdate()
  {
    // Arrange
    Cart cart = Cart.One();
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Cart, Guid> repository = scope.Repository<Cart, Guid>();
    await repository.InsertAsync(cart);

    // Act
    cart.Items.RemoveAt(0);
    await repository.UpdateAsync(cart);

    // Assert
    Cart? updated = await repository.FindAsync(cart.Id);
    updated!.Items.Should().HaveCount(cart.Items.Count);
  }

  #endregion
}
