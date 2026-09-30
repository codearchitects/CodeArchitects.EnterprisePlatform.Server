using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// The demo of a domain model persisted through a different document model, mapped by Mapster.
/// Every operation must behave as on the direct repository: same transactions, same concurrency checks.
/// </summary>
[Xunit.Collection(TestCollection.Name)]
public class MapsterRepositoryTests(TestFixture fixture) : TestBase(fixture)
{
  private IMongoCollection<BsonDocument> RawPurchaseOrders()
  {
    string name = _fixture.GetCollection<PurchaseOrderDocument>().CollectionNamespace.CollectionName;
    return _fixture.GetCollection<PurchaseOrderDocument>().Database.GetCollection<BsonDocument>(name);
  }

  private Task<long> CountPurchaseOrdersAsync() =>
    _fixture.GetCollection<PurchaseOrderDocument>().CountDocumentsAsync(FilterDefinition<PurchaseOrderDocument>.Empty);

  #region Shape

  [Fact]
  public async Task InsertAsync_ShouldPersistTheDocumentShape()
  {
    // Arrange
    PurchaseOrder order = PurchaseOrder.One();
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    await repository.InsertAsync(order);

    // Assert
    // What is stored is the document model, not the domain one.
    BsonDocument document = await RawPurchaseOrders()
      .Find(Builders<BsonDocument>.Filter.Eq("_id", order.Id)).SingleAsync();

    document["Code"].AsString.Should().Be(order.Number.Value);
    document["TotalAmount"].ToDecimal().Should().Be(order.Total.Amount);
    document["TotalCurrency"].AsString.Should().Be(order.Total.Currency);
    document["Shipping"]["Line1"].AsString.Should().Be(order.ShippingAddress.Street);
    document["Shipping"]["Zip"].AsString.Should().Be(order.ShippingAddress.PostalCode);
    document["Lines"].AsBsonArray.Should().HaveCount(order.Lines.Count);
    document["Lines"][0]["Qty"].AsInt32.Should().Be(order.Lines[0].Quantity);
    document["Lines"][0]["Price"].ToDecimal().Should().Be(order.Lines[0].UnitPrice.Amount);
    document["CustomerId"].AsGuid.Should().Be(order.CustomerId);
    document["SchemaVersion"].AsInt32.Should().Be(PurchaseOrderMapping.CurrentSchemaVersion);
    document.Contains("Number").Should().BeFalse();
    document.Contains("ShippingAddress").Should().BeFalse();
  }

  [Fact]
  public async Task FindAsync_ShouldRebuildTheAggregate()
  {
    // Arrange
    PurchaseOrder expected = PurchaseOrder.One();
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();
    await repository.InsertAsync(expected);

    // Act
    PurchaseOrder? order = await repository.FindAsync(expected.Id);

    // Assert
    // Key, value objects and embedded lines come back intact.
    order.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Find_ShouldRebuildTheAggregate()
  {
    // Arrange
    PurchaseOrder expected = PurchaseOrder.One();
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();
    repository.Insert(expected);

    // Act
    PurchaseOrder? order = repository.Find(expected.Id);

    // Assert
    order.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public async Task FindAsync_ShouldReturnNull_WhenTheDocumentDoesNotExist()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act & Assert
    (await repository.FindAsync(Guid.NewGuid())).Should().BeNull();
  }

  [Fact]
  public async Task FindAsync_ShouldThrow_WhenAnIncludeIsRequested()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    Func<Task> act = () => repository.FindAsync(Guid.NewGuid(), include => include.Include(order => order.Lines));

    // Assert
    await act.Should().ThrowAsync<NotSupportedException>().WithMessage("*Find(key)*");
  }

  #endregion

  #region Writes

  [Fact]
  public async Task UpdateAsync_ShouldReplaceTheDocument()
  {
    // Arrange
    PurchaseOrder order = PurchaseOrder.One();
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();
    await repository.InsertAsync(order);

    // Act
    order.ShippingAddress = order.ShippingAddress with { City = "Roma" };
    order.Lines.RemoveAt(1);
    await repository.UpdateAsync(order);

    // Assert
    PurchaseOrder? updated = await repository.FindAsync(order.Id);
    updated.Should().BeEquivalentTo(order);
  }

  [Fact]
  public async Task UpdateAsync_ShouldThrow_WhenTheDocumentDoesNotExist()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    Func<Task> act = () => repository.UpdateAsync(PurchaseOrder.One());

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>().WithMessage("*PurchaseOrderDocument*");
  }

  [Fact]
  public async Task UpsertAsync_ShouldInsert_ThenReplace()
  {
    // Arrange
    PurchaseOrder order = PurchaseOrder.One();
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    await repository.UpsertAsync(order);
    order.Number = new PurchaseOrderNumber("PO-UPSERTED");
    await repository.UpsertAsync(order);

    // Assert
    (await CountPurchaseOrdersAsync()).Should().Be(1);
    (await repository.FindAsync(order.Id))!.Number.Value.Should().Be("PO-UPSERTED");
  }

  [Fact]
  public async Task RemoveAsync_ShouldDeleteTheDocument_ByEntity()
  {
    // Arrange
    PurchaseOrder order = PurchaseOrder.One();
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();
    await repository.InsertAsync(order);

    // Act
    await repository.RemoveAsync(order);

    // Assert
    (await repository.FindAsync(order.Id)).Should().BeNull();
  }

  [Fact]
  public async Task RemoveAsync_ShouldDeleteTheDocument_ByKey()
  {
    // Arrange
    PurchaseOrder order = PurchaseOrder.One();
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();
    await repository.InsertAsync(order);

    // Act
    await repository.RemoveAsync(order.Id);

    // Assert
    (await repository.FindAsync(order.Id)).Should().BeNull();
  }

  [Fact]
  public async Task RemoveAsync_ShouldThrow_WhenTheDocumentDoesNotExist()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    Func<Task> act = () => repository.RemoveAsync(PurchaseOrder.One());

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();
  }

  [Fact]
  public async Task InsertManyAsync_AndUpdateManyAsync_ShouldMapEveryElement()
  {
    // Arrange
    PurchaseOrder[] orders = [PurchaseOrder.One(), PurchaseOrder.One(), PurchaseOrder.One()];
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    await repository.InsertManyAsync(orders);
    foreach (PurchaseOrder order in orders)
    {
      order.Total = order.Total with { Currency = "USD" };
    }
    await repository.UpdateManyAsync(orders);

    // Assert
    foreach (PurchaseOrder order in orders)
    {
      (await repository.FindAsync(order.Id)).Should().BeEquivalentTo(order);
    }
  }

  [Fact]
  public async Task InsertManyAsync_ShouldDoNothing_WhenTheSequenceIsEmpty()
  {
    // Arrange
    using TestScope scope = _fixture.CreateStandaloneScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    // On the standalone server a round-trip would fail: no transaction is available.
    Func<Task> act = () => repository.InsertManyAsync([]);

    // Assert
    await act.Should().NotThrowAsync();
  }

  [Fact]
  public async Task UpdateManyAsync_ShouldThrowAndPersistNothing_WhenOneDocumentDoesNotExist()
  {
    // Arrange
    PurchaseOrder existing = PurchaseOrder.One();
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();
    await repository.InsertAsync(existing);

    // Act
    existing.Number = new PurchaseOrderNumber("PO-CHANGED");
    Func<Task> act = () => repository.UpdateManyAsync([existing, PurchaseOrder.One()]);

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();
    (await repository.FindAsync(existing.Id))!.Number.Value.Should().NotBe("PO-CHANGED");
  }

  [Fact]
  public async Task InsertManyAsync_ShouldThrow_OnAStandaloneServer()
  {
    // Arrange
    using TestScope scope = _fixture.CreateStandaloneScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    Func<Task> act = () => repository.InsertManyAsync([PurchaseOrder.One(), PurchaseOrder.One()]);

    // Assert
    await act.Should().ThrowAsync<TransactionsNotSupportedException>();
  }

  #endregion

  #region Unit of work

  [Fact]
  public async Task UnitOfWork_ShouldDeferMappedWrites_UntilSave()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    await using IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin();
    await repository.InsertAsync(PurchaseOrder.One());
    await repository.InsertManyAsync([PurchaseOrder.One(), PurchaseOrder.One()]);

    // Assert
    (await CountPurchaseOrdersAsync()).Should().Be(0);

    await unitOfWork.SaveAsync();
    (await CountPurchaseOrdersAsync()).Should().Be(3);
  }

  [Fact]
  public async Task UnitOfWork_ShouldPersistNothing_WhenNotSaved()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();

    // Act
    await using (IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin())
    {
      await repository.InsertAsync(PurchaseOrder.One());
    }

    // Assert
    (await CountPurchaseOrdersAsync()).Should().Be(0);
  }

  [Fact]
  public async Task UnitOfWork_ShouldRollBackTheMappedWrites_WhenOneFails()
  {
    // Arrange
    PurchaseOrder existing = PurchaseOrder.One();
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();
    await repository.InsertAsync(existing);

    // Act
    Func<Task> act = async () =>
    {
      await using IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin();
      await repository.InsertAsync(PurchaseOrder.One());
      await repository.UpdateAsync(PurchaseOrder.One());   // does not exist
      await unitOfWork.SaveAsync();
    };

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>();
    (await CountPurchaseOrdersAsync()).Should().Be(1);
  }

  [Fact]
  public async Task CustomQuery_ShouldReturnDomainEntities_AndUseTheSession()
  {
    // Arrange
    Guid customerId = Guid.NewGuid();
    PurchaseOrder first = PurchaseOrder.One(customerId);
    PurchaseOrder second = PurchaseOrder.One(customerId);
    using (TestScope seeding = _fixture.CreateScope())
    {
      await seeding.Get<PurchaseOrderRepository>().InsertManyAsync([first, second, PurchaseOrder.One()]);
    }

    // A fresh scope: its session has not run any operation yet.
    using TestScope scope = _fixture.CreateScope();
    PurchaseOrderRepository repository = scope.Get<PurchaseOrderRepository>();
    scope.Context.Session.OperationTime.Should().BeNull();

    // Act
    List<PurchaseOrder> orders = await repository.FindByCustomerAsync(customerId);

    // Assert
    orders.Should().BeEquivalentTo([first, second]);
    // The server returns an operation time only to the session the query ran on: a query issued
    // without Session would run on an implicit one and leave this session untouched.
    scope.Context.Session.OperationTime.Should().NotBeNull();
  }

  #endregion

  #region Generated keys

  [Fact]
  public async Task InsertAsync_ShouldGiveTheGeneratedKeyToTheEntity()
  {
    // Arrange
    Ticket ticket = new() { Title = "Printer out of paper" };
    using TestScope scope = _fixture.CreateScope();
    IRepository<Ticket, string> repository = scope.Get<IRepository<Ticket, string>>();

    // Act
    await repository.InsertAsync(ticket);

    // Assert
    ObjectId.TryParse(ticket.Id, out _).Should().BeTrue();
    (await repository.FindAsync(ticket.Id!)).Should().BeEquivalentTo(ticket);
  }

  [Fact]
  public async Task InsertAsync_ShouldGiveTheGeneratedKeyToTheEntity_BeforeTheUnitOfWorkCommits()
  {
    // Arrange
    Ticket first = new() { Title = "First" };
    Ticket second = new() { Title = "Second" };
    using TestScope scope = _fixture.CreateScope();
    IRepository<Ticket, string> repository = scope.Get<IRepository<Ticket, string>>();

    // Act
    await using (IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin())
    {
      await repository.InsertAsync(first);
      await repository.InsertManyAsync([second]);

      // Assert
      // The write is only queued, but the entity already has its identity.
      first.Id.Should().NotBeNullOrEmpty();
      second.Id.Should().NotBeNullOrEmpty().And.NotBe(first.Id);

      await unitOfWork.SaveAsync();
    }

    (await repository.FindAsync(first.Id!)).Should().BeEquivalentTo(first);
    (await repository.FindAsync(second.Id!)).Should().BeEquivalentTo(second);
  }

  #endregion
}
