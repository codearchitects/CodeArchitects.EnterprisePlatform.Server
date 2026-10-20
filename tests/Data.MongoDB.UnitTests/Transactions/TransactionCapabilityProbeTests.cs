using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.Transactions;

public class TransactionCapabilityProbeTests
{
  private readonly Mock<IMongoDatabase> _database = new();

  private TransactionCapabilityProbe CreateSut() =>
    new(_database.Object, NullLogger<TransactionCapabilityProbe>.Instance);

  private void SetupHandshake(params BsonDocument[] responses)
  {
    var sequence = _database.SetupSequence(database => database.RunCommand(
      It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()));

    foreach (BsonDocument response in responses)
    {
      sequence = sequence.Returns(response);
    }
  }

  private void VerifyHandshakeCount(int times) =>
    _database.Verify(
      database => database.RunCommand(
        It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()),
      Times.Exactly(times));

  [Fact]
  public void AreTransactionsSupported_ShouldReturnTrue_WhenTheServerIsAReplicaSetMember()
  {
    // Arrange
    SetupHandshake(new BsonDocument { { "isWritablePrimary", true }, { "setName", "rs0" } });

    // Act
    bool supported = CreateSut().AreTransactionsSupported();

    // Assert
    supported.Should().BeTrue();
  }

  [Fact]
  public void AreTransactionsSupported_ShouldReturnTrue_WhenTheServerIsAMongosRouter()
  {
    // Arrange
    SetupHandshake(new BsonDocument { { "isWritablePrimary", true }, { "msg", "isdbgrid" } });

    // Act
    bool supported = CreateSut().AreTransactionsSupported();

    // Assert
    supported.Should().BeTrue();
  }

  [Fact]
  public void AreTransactionsSupported_ShouldCacheTheVerdict_WhenTheServerIsStandalone()
  {
    // Arrange
    SetupHandshake(new BsonDocument { { "isWritablePrimary", true } });
    TransactionCapabilityProbe sut = CreateSut();

    // Act
    bool first = sut.AreTransactionsSupported();
    bool second = sut.AreTransactionsSupported();

    // Assert
    first.Should().BeFalse();
    second.Should().BeFalse();
    VerifyHandshakeCount(1);
  }

  [Fact]
  public void AreTransactionsSupported_ShouldProbeAgain_WhenTheReplicaSetHasNotBeenInitiatedYet()
  {
    // Arrange
    // A mongod started with --replSet before rs.initiate: it knows it is a replica set member,
    // but it has no set name yet. Transactions become available as soon as the set is initiated.
    SetupHandshake(
      new BsonDocument { { "isWritablePrimary", false }, { "secondary", false }, { "isreplicaset", true } },
      new BsonDocument { { "isWritablePrimary", true }, { "setName", "rs0" } });
    TransactionCapabilityProbe sut = CreateSut();

    // Act
    bool beforeInitiate = sut.AreTransactionsSupported();
    bool afterInitiate = sut.AreTransactionsSupported();

    // Assert
    beforeInitiate.Should().BeFalse();
    afterInitiate.Should().BeTrue();
  }

  [Fact]
  public async Task AreTransactionsSupportedAsync_ShouldReturnTrue_WhenTheServerIsAReplicaSetMember()
  {
    // Arrange
    _database
      .Setup(database => database.RunCommandAsync(
        It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new BsonDocument { { "isWritablePrimary", true }, { "setName", "rs0" } });

    // Act
    bool supported = await CreateSut().AreTransactionsSupportedAsync(CancellationToken.None);

    // Assert
    supported.Should().BeTrue();
    _database.Verify(
      database => database.RunCommand(
        It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Fact]
  public async Task AreTransactionsSupportedAsync_ShouldShareTheCachedVerdict_WithTheSynchronousProbe()
  {
    // Arrange
    SetupHandshake(new BsonDocument { { "isWritablePrimary", true } });
    TransactionCapabilityProbe sut = CreateSut();

    // Act
    bool first = sut.AreTransactionsSupported();
    bool second = await sut.AreTransactionsSupportedAsync(CancellationToken.None);

    // Assert
    first.Should().BeFalse();
    second.Should().BeFalse();
    VerifyHandshakeCount(1);
    _database.Verify(
      database => database.RunCommandAsync(
        It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Fact]
  public async Task AreTransactionsSupportedAsync_ShouldProbeAgain_WhenTheHandshakeFailed()
  {
    // Arrange
    _database.SetupSequence(database => database.RunCommandAsync(
        It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()))
      .ThrowsAsync(new MongoClientException("hello failed"))
      .ThrowsAsync(new MongoClientException("isMaster failed"))
      .ReturnsAsync(new BsonDocument { { "isWritablePrimary", true }, { "setName", "rs0" } });
    TransactionCapabilityProbe sut = CreateSut();

    // Act
    bool whileFailing = await sut.AreTransactionsSupportedAsync(CancellationToken.None);
    bool afterRecovery = await sut.AreTransactionsSupportedAsync(CancellationToken.None);

    // Assert
    whileFailing.Should().BeFalse();
    afterRecovery.Should().BeTrue();
  }

  [Fact]
  public void AreTransactionsSupported_ShouldProbeAgain_WhenTheHandshakeFailed()
  {
    // Arrange
    _database.SetupSequence(database => database.RunCommand(
        It.IsAny<Command<BsonDocument>>(), It.IsAny<ReadPreference>(), It.IsAny<CancellationToken>()))
      .Throws(new MongoClientException("hello failed"))
      .Throws(new MongoClientException("isMaster failed"))
      .Returns(new BsonDocument { { "isWritablePrimary", true }, { "setName", "rs0" } });
    TransactionCapabilityProbe sut = CreateSut();

    // Act
    bool whileFailing = sut.AreTransactionsSupported();
    bool afterRecovery = sut.AreTransactionsSupported();

    // Assert
    whileFailing.Should().BeFalse();
    afterRecovery.Should().BeTrue();
  }
}
