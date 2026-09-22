using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

[Xunit.Collection(TestCollection.Name)]
public class UnitOfWorkTests(TestFixture fixture) : TestBase(fixture)
{
  [Fact]
  public void Context_ShouldExposeTheSameSession_WithinAScope()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();

    // Act
    IClientSessionHandle first = scope.Context.Session;
    IClientSessionHandle second = scope.Context.Session;

    // Assert
    // Every operation of the scope — reads included — must run on this one session,
    // otherwise it cannot take part in the transaction opened at commit time.
    second.Should().BeSameAs(first);
  }

  [Fact]
  public void Context_ShouldExposeADifferentSession_PerScope()
  {
    // Arrange
    using TestScope first = _fixture.CreateScope();
    using TestScope second = _fixture.CreateScope();

    // Act & Assert
    second.Context.Session.Should().NotBeSameAs(first.Context.Session);
  }

  [Fact]
  public async Task UnitOfWork_ShouldCommitEveryOperation_WhenSaveSucceeds()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Customer, Guid> repository = scope.Repository<Customer, Guid>();
    Customer first = Customer.One();
    Customer second = Customer.One();

    // Act
    await using (IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin())
    {
      await repository.InsertAsync(first);
      await repository.InsertAsync(second);

      await unitOfWork.SaveAsync();
    }

    // Assert
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();
    (await collection.CountDocumentsAsync(FilterDefinition<Customer>.Empty)).Should().Be(2);
  }

  [Fact]
  public async Task UnitOfWork_ShouldDeferEveryWrite_UntilSave()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Customer, Guid> repository = scope.Repository<Customer, Guid>();
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();

    // Act
    await using IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin();
    await repository.InsertAsync(Customer.One());

    // Assert
    // Inside a unit of work the write is queued, not applied: nothing is visible yet.
    (await collection.CountDocumentsAsync(FilterDefinition<Customer>.Empty)).Should().Be(0);

    await unitOfWork.SaveAsync();
    (await collection.CountDocumentsAsync(FilterDefinition<Customer>.Empty)).Should().Be(1);
  }

  [Fact]
  public async Task UnitOfWork_ShouldRollBackEveryOperation_WhenOneFails()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Customer, Guid> repository = scope.Repository<Customer, Guid>();
    Customer valid = Customer.One();
    Customer duplicate = Customer.One();

    // Act
    Func<Task> act = async () =>
    {
      await using IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin();

      await repository.InsertAsync(valid);
      await repository.InsertAsync(duplicate);
      await repository.InsertAsync(duplicate);   // duplicate key: fails at commit

      await unitOfWork.SaveAsync();
    };

    // Assert
    await act.Should().ThrowAsync<MongoException>();

    // This is the test the previous implementation did not have: the transaction was opened on a
    // session no operation used, so the writes went through anyway and nothing rolled back.
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();
    (await collection.CountDocumentsAsync(FilterDefinition<Customer>.Empty)).Should().Be(0);
  }

  [Fact]
  public async Task UnitOfWork_ShouldNotReplayOperations_WhenSaveIsCalledTwice()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Customer, Guid> repository = scope.Repository<Customer, Guid>();

    // Act
    await using IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin();
    await repository.InsertAsync(Customer.One());
    await unitOfWork.SaveAsync();
    await unitOfWork.SaveAsync();

    // Assert
    // The queue is drained before the commit: a second Save must not rewrite the same document.
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();
    (await collection.CountDocumentsAsync(FilterDefinition<Customer>.Empty)).Should().Be(1);
  }

  [Fact]
  public async Task UnitOfWork_ShouldCommitOnDispose_WhenAutoSaveIsOn()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Customer, Guid> repository = scope.Repository<Customer, Guid>();

    // Act
    await using (IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin(autoSave: true))
    {
      await repository.InsertAsync(Customer.One());
      await repository.InsertAsync(Customer.One());
    }

    // Assert
    IMongoCollection<Customer> collection = _fixture.GetCollection<Customer>();
    (await collection.CountDocumentsAsync(FilterDefinition<Customer>.Empty)).Should().Be(2);
  }
}
