using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Driver;
using System.Data;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Every operation is exercised through both its synchronous and its asynchronous variant,
/// against the same assertions: the two paths must behave the same.
/// </summary>
[Xunit.Collection(TestCollection.Name)]
public class RepositoryTests(TestFixture fixture) : TestBase(fixture)
{
  private static MongoDBRepository<Customer, Guid> Repository(TestScope scope) => scope.Repository<Customer, Guid>();

  private static Task Run(bool async, Action sync, Func<Task> asyncOperation)
  {
    if (async)
      return asyncOperation();

    sync();
    return Task.CompletedTask;
  }

  private static async Task<T> Run<T>(bool async, Func<T> sync, Func<Task<T>> asyncOperation) =>
    async ? await asyncOperation() : sync();

  private Task<Customer?> Persisted(Guid id) =>
    _fixture.GetCollection<Customer>().Find(x => x.Id == id).FirstOrDefaultAsync()!;

  private Task<List<Customer>> AllPersisted() =>
    _fixture.GetCollection<Customer>().Find(FilterDefinition<Customer>.Empty).ToListAsync();

  #region Find

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Find_ShouldReturnEntity_WhenEntityExists(bool async)
  {
    // Arrange
    Customer expected = Customer.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Customer? customer = await Run(async,
      () => Repository(scope).Find(expected.Id),
      () => Repository(scope).FindAsync(expected.Id));

    // Assert
    customer.Should().BeEquivalentTo(expected);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Find_ShouldReturnNull_WhenEntityDoesNotExist(bool async)
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();

    // Act
    Customer? customer = await Run(async,
      () => Repository(scope).Find(Guid.NewGuid()),
      () => Repository(scope).FindAsync(Guid.NewGuid()));

    // Assert
    customer.Should().BeNull();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Find_ShouldReturnEntity_WhenNothingIsIncluded(bool async)
  {
    // Arrange
    Customer expected = Customer.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Customer? customer = await Run(async,
      () => Repository(scope).Find(expected.Id, _ => { }),
      () => Repository(scope).FindAsync(expected.Id, _ => { }));

    // Assert
    customer.Should().BeEquivalentTo(expected);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Find_ShouldReturnTheWholeAggregate_WhenIncludingAnEmbeddedNavigation(bool async)
  {
    // Arrange
    Cart expected = Cart.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Cart, Guid> repository = scope.Repository<Cart, Guid>();

    // Act
    Cart? cart = await Run(async,
      () => repository.Find(expected.Id, include => include.Include(c => c.Items)),
      () => repository.FindAsync(expected.Id, include => include.Include(c => c.Items)));

    // Assert
    // As EF Core does with owned types: the items are in the document, so the include is
    // accepted and loads nothing more than Find(key) does.
    cart.Should().BeEquivalentTo(expected);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Find_ShouldThrow_WhenIncludingAReferenceToAnotherCollection(bool async)
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Invoice, Guid> repository = scope.Repository<Invoice, Guid>();

    // Act
    Func<Task> act = () => Run(async,
      () => repository.Find(Guid.NewGuid(), include => include.Include(invoice => invoice.Customer)),
      () => repository.FindAsync(Guid.NewGuid(), include => include.Include(invoice => invoice.Customer)));

    // Assert
    // No silent degradation: the reference is not resolved, so the include is rejected rather
    // than ignored, even when there is no document to return.
    await act.Should().ThrowAsync<NotSupportedException>().WithMessage("*Invoice.Customer*");
  }

  #endregion

  #region Insert

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Insert_ShouldInsertEntity_WhenEntityDoesNotExist(bool async)
  {
    // Arrange
    Customer expected = Customer.One();
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Run(async,
      () => Repository(scope).Insert(expected),
      () => Repository(scope).InsertAsync(expected));

    // Assert
    (await Persisted(expected.Id)).Should().BeEquivalentTo(expected);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Insert_ShouldThrow_WhenEntityAlreadyExists(bool async)
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).Insert(existing),
      () => Repository(scope).InsertAsync(existing));

    // Assert
    await act.Should().ThrowAsync<MongoWriteException>();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task InsertMany_ShouldInsertEveryEntity(bool async)
  {
    // Arrange
    Customer[] expected = [Customer.One(), Customer.One(), Customer.One()];
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Run(async,
      () => Repository(scope).InsertMany(expected),
      () => Repository(scope).InsertManyAsync(expected));

    // Assert
    (await AllPersisted()).Should().BeEquivalentTo(expected);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task InsertMany_ShouldThrowAndInsertNothing_WhenOneEntityAlreadyExists(bool async)
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    Customer fresh = Customer.One();
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).InsertMany([fresh, existing]),
      () => Repository(scope).InsertManyAsync([fresh, existing]));

    // Assert
    await act.Should().ThrowAsync<MongoBulkWriteException>();
    (await Persisted(fresh.Id)).Should().BeNull("the insert of several documents runs inside a transaction");
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task InsertMany_ShouldDoNothing_WhenTheSequenceIsEmpty(bool async)
  {
    // Arrange
    // The standalone server rejects transactions: an empty batch must not even ask for one.
    using TestScope scope = _fixture.CreateStandaloneScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).InsertMany([]),
      () => Repository(scope).InsertManyAsync([]));

    // Assert
    await act.Should().NotThrowAsync();
  }

  #endregion

  #region Update

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Update_ShouldUpdateEntity_WhenEntityExists(bool async)
  {
    // Arrange
    Customer expected = Customer.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    expected.Name = "ModifiedName";
    await Run(async,
      () => Repository(scope).Update(expected),
      () => Repository(scope).UpdateAsync(expected));

    // Assert
    (await Persisted(expected.Id)).Should().BeEquivalentTo(expected);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Update_ShouldNotThrow_WhenNothingChanged(bool async)
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).Update(existing),
      () => Repository(scope).UpdateAsync(existing));

    // Assert
    // A replace that matches but rewrites nothing reports ModifiedCount == 0. The success
    // criterion is MatchedCount, otherwise an idempotent update would look like a conflict.
    await act.Should().NotThrowAsync();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Update_ShouldThrow_WhenEntityDoesNotExist(bool async)
  {
    // Arrange
    Customer missing = Customer.One();
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).Update(missing),
      () => Repository(scope).UpdateAsync(missing));

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>().WithMessage($"*{missing.Id}*");
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task UpdateMany_ShouldUpdateEveryEntity(bool async)
  {
    // Arrange
    Customer[] seed = [Customer.One(), Customer.One()];
    await _fixture.SeedAsync(seed);
    using TestScope scope = _fixture.CreateScope();

    // Act
    seed[0].Name = "First";
    seed[1].Name = "Second";
    await Run(async,
      () => Repository(scope).UpdateMany(seed),
      () => Repository(scope).UpdateManyAsync(seed));

    // Assert
    (await AllPersisted()).Select(customer => customer.Name).Should().BeEquivalentTo(["First", "Second"]);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task UpdateMany_ShouldNotThrow_WhenNothingChanged(bool async)
  {
    // Arrange
    Customer[] seed = [Customer.One(), Customer.One()];
    await _fixture.SeedAsync(seed);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).UpdateMany(seed),
      () => Repository(scope).UpdateManyAsync(seed));

    // Assert
    await act.Should().NotThrowAsync();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task UpdateMany_ShouldThrowAndApplyNothing_WhenOneEntityDoesNotExist(bool async)
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    existing.Name = "Modified";
    Func<Task> act = () => Run(async,
      () => Repository(scope).UpdateMany([existing, Customer.One()]),
      () => Repository(scope).UpdateManyAsync([existing, Customer.One()]));

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();
    (await Persisted(existing.Id))!.Name.Should().NotBe("Modified", "the bulk write runs inside a transaction");
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task UpdateMany_ShouldDoNothing_WhenTheSequenceIsEmpty(bool async)
  {
    // Arrange
    using TestScope scope = _fixture.CreateStandaloneScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).UpdateMany([]),
      () => Repository(scope).UpdateManyAsync([]));

    // Assert
    await act.Should().NotThrowAsync();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task UpdateMany_ShouldThrow_WhenAnElementIsNull(bool async)
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).UpdateMany([Customer.One(), null!]),
      () => Repository(scope).UpdateManyAsync([Customer.One(), null!]));

    // Assert
    await act.Should().ThrowAsync<ArgumentException>();
  }

  #endregion

  #region Upsert

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Upsert_ShouldUpdateEntity_WhenEntityExists(bool async)
  {
    // Arrange
    Customer expected = Customer.One();
    await _fixture.SeedAsync([expected]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    expected.Name = "ModifiedName";
    await Run(async,
      () => Repository(scope).Upsert(expected),
      () => Repository(scope).UpsertAsync(expected));

    // Assert
    (await Persisted(expected.Id)).Should().BeEquivalentTo(expected);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Upsert_ShouldInsertEntity_WhenEntityDoesNotExist(bool async)
  {
    // Arrange
    Customer expected = Customer.One();
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Run(async,
      () => Repository(scope).Upsert(expected),
      () => Repository(scope).UpsertAsync(expected));

    // Assert
    (await Persisted(expected.Id)).Should().BeEquivalentTo(expected);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Upsert_ShouldNotThrow_WhenNothingChanged(bool async)
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).Upsert(existing),
      () => Repository(scope).UpsertAsync(existing));

    // Assert
    await act.Should().NotThrowAsync();
  }

  #endregion

  #region Remove

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Remove_ShouldRemoveEntity_WhenEntityExists(bool async)
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Run(async,
      () => Repository(scope).Remove(existing),
      () => Repository(scope).RemoveAsync(existing));

    // Assert
    (await Persisted(existing.Id)).Should().BeNull();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Remove_ShouldRemoveEntity_WhenEntityKeyExists(bool async)
  {
    // Arrange
    Customer existing = Customer.One();
    await _fixture.SeedAsync([existing]);
    using TestScope scope = _fixture.CreateScope();

    // Act
    await Run(async,
      () => Repository(scope).Remove(existing.Id),
      () => Repository(scope).RemoveAsync(existing.Id));

    // Assert
    (await Persisted(existing.Id)).Should().BeNull();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Remove_ShouldThrow_WhenEntityDoesNotExist(bool async)
  {
    // Arrange
    Customer missing = Customer.One();
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).Remove(missing),
      () => Repository(scope).RemoveAsync(missing));

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>().WithMessage($"*{missing.Id}*");
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Remove_ShouldThrow_WhenEntityKeyDoesNotExist(bool async)
  {
    // Arrange
    Guid missing = Guid.NewGuid();
    using TestScope scope = _fixture.CreateScope();

    // Act
    Func<Task> act = () => Run(async,
      () => Repository(scope).Remove(missing),
      () => Repository(scope).RemoveAsync(missing));

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>().WithMessage($"*{missing}*");
  }

  #endregion

  #region Cancellation

  [Fact]
  public async Task InsertAsync_ShouldThrowAndWriteNothing_WhenTheTokenIsAlreadyCancelled()
  {
    // Arrange
    Customer customer = Customer.One();
    using TestScope scope = _fixture.CreateScope();
    using CancellationTokenSource cancellation = new();
    cancellation.Cancel();

    // Act
    Func<Task> act = () => Repository(scope).InsertAsync(customer, cancellation.Token);

    // Assert
    await act.Should().ThrowAsync<OperationCanceledException>();
    (await Persisted(customer.Id)).Should().BeNull();
  }

  #endregion

  #region Registration

  [Fact]
  public async Task Repository_ShouldWork_WhenResolvedThroughTheCaepAbstraction()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    IRepository<Customer, Guid> repository = scope.Get<IRepository<Customer, Guid>>();
    Customer customer = Customer.One();

    // Act
    await repository.InsertAsync(customer);
    customer.Name = "Modified";
    await repository.UpdateAsync(customer);
    Customer? found = await repository.FindAsync(customer.Id);
    await repository.RemoveAsync(customer.Id);

    // Assert
    found.Should().BeEquivalentTo(customer);
    (await repository.FindAsync(customer.Id)).Should().BeNull();
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
