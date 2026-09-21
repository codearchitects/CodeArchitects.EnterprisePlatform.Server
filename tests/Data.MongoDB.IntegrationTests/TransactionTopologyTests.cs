using CodeArchitects.Platform.Data.MongoDB.Fixtures;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Verifies that the provider tells the truth about transactions instead of silently running
/// without them.
/// </summary>
[Xunit.Collection(TestCollection.Name)]
public class TransactionTopologyTests(TestFixture fixture) : TestBase(fixture)
{
  [Fact]
  public async Task SingleWrite_ShouldSucceed_OnAStandaloneServer()
  {
    // Arrange
    using TestScope scope = _fixture.CreateStandaloneScope();
    MongoDBRepository<Customer, Guid> repository = scope.Repository<Customer, Guid>();

    // Act
    Func<Task> act = () => repository.InsertAsync(Customer.One());

    // Assert
    // A write on a single document is already atomic: no transaction is needed, so the topology
    // never comes into play.
    await act.Should().NotThrowAsync();
  }

  [Fact]
  public async Task MultipleWrites_ShouldThrow_OnAStandaloneServer()
  {
    // Arrange
    using TestScope scope = _fixture.CreateStandaloneScope();
    MongoDBRepository<Customer, Guid> repository = scope.Repository<Customer, Guid>();

    // Act
    Func<Task> act = () => repository.InsertManyAsync([Customer.One(), Customer.One()]);

    // Assert
    // InsertMany is only atomic inside a transaction, and a standalone server cannot provide
    // one. With TransactionMode.Required the provider refuses rather than degrading silently.
    await act.Should().ThrowAsync<TransactionsNotSupportedException>();
  }

  [Fact]
  public async Task UnitOfWork_ShouldThrow_OnAStandaloneServer()
  {
    // Arrange
    using TestScope scope = _fixture.CreateStandaloneScope();
    MongoDBRepository<Customer, Guid> repository = scope.Repository<Customer, Guid>();

    // Act
    Func<Task> act = async () =>
    {
      await using IUnitOfWork unitOfWork = scope.UnitOfWorkManager.Begin();

      await repository.InsertAsync(Customer.One());
      await repository.InsertAsync(Customer.One());

      await unitOfWork.SaveAsync();
    };

    // Assert
    await act.Should().ThrowAsync<TransactionsNotSupportedException>();
  }

  [Fact]
  public async Task MultipleWrites_ShouldSucceed_OnAReplicaSet()
  {
    // Arrange
    using TestScope scope = _fixture.CreateScope();
    MongoDBRepository<Customer, Guid> repository = scope.Repository<Customer, Guid>();

    // Act
    Func<Task> act = () => repository.InsertManyAsync([Customer.One(), Customer.One()]);

    // Assert
    await act.Should().NotThrowAsync();
  }
}
