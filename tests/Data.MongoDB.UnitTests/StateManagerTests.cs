using CodeArchitects.Platform.Data.MongoDB.Transactions;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

public class StateManagerTests
{
  private readonly Mock<IMongoClient> _client = new();
  private readonly Mock<IClientSessionHandle> _session = new();
  private readonly Mock<ITransactionCapabilityProbe> _probe = new();

  public StateManagerTests()
  {
    _client
      .Setup(client => client.StartSession(It.IsAny<ClientSessionOptions>(), It.IsAny<CancellationToken>()))
      .Returns(_session.Object);
  }

  private StateManager CreateSut(TransactionMode mode = TransactionMode.Required) =>
    new(_client.Object, _probe.Object, new MongoDBOptions { TransactionMode = mode },
        NullLogger<StateManager>.Instance);

  private static Execution Recording(List<IClientSessionHandle> sessions) =>
    new(sessions.Add, (session, _) => { sessions.Add(session); return Task.CompletedTask; });

  [Fact]
  public void Save_ShouldNotOpenASession_WhenThereIsNothingToWrite()
  {
    // Arrange
    StateManager sut = CreateSut();

    // Act
    sut.Save();

    // Assert
    _client.Verify(
      client => client.StartSession(It.IsAny<ClientSessionOptions>(), It.IsAny<CancellationToken>()),
      Times.Never);
  }

  [Fact]
  public void Save_ShouldNotUseATransaction_WhenThereIsASingleOperation()
  {
    // Arrange
    StateManager sut = CreateSut();
    List<IClientSessionHandle> sessions = new();
    sut.AddExecution(Recording(sessions), requiresTransaction: false);

    // Act
    sut.Save();

    // Assert
    // A single-document write is already atomic: the topology is not even consulted.
    sessions.Should().ContainSingle().Which.Should().BeSameAs(_session.Object);
    _probe.Verify(probe => probe.AreTransactionsSupported(), Times.Never);
  }

  [Fact]
  public void Save_ShouldRunAllOperationsOnTheSameSession()
  {
    // Arrange
    StateManager sut = CreateSut(TransactionMode.Disabled);
    List<IClientSessionHandle> sessions = new();
    sut.AddExecution(Recording(sessions), requiresTransaction: false);
    sut.AddExecution(Recording(sessions), requiresTransaction: false);

    // Act
    sut.Save();

    // Assert
    sessions.Should().HaveCount(2).And.OnlyContain(session => session == _session.Object);
  }

  [Fact]
  public void Save_ShouldThrow_WhenATransactionIsRequiredButTheTopologyDoesNotSupportIt()
  {
    // Arrange
    _probe.Setup(probe => probe.AreTransactionsSupported()).Returns(false);
    StateManager sut = CreateSut(TransactionMode.Required);
    List<IClientSessionHandle> sessions = new();
    sut.AddExecution(Recording(sessions), requiresTransaction: true);

    // Act
    Action act = sut.Save;

    // Assert
    act.Should().Throw<TransactionsNotSupportedException>();
    sessions.Should().BeEmpty();
  }

  [Fact]
  public void Save_ShouldDegrade_WhenTransactionsAreOnlyUsedWhenSupported()
  {
    // Arrange
    _probe.Setup(probe => probe.AreTransactionsSupported()).Returns(false);
    StateManager sut = CreateSut(TransactionMode.WhenSupported);
    List<IClientSessionHandle> sessions = new();
    sut.AddExecution(Recording(sessions), requiresTransaction: true);

    // Act
    sut.Save();

    // Assert
    sessions.Should().ContainSingle();
  }

  [Fact]
  public void Save_ShouldNotReplayOperations_WhenCalledTwice()
  {
    // Arrange
    StateManager sut = CreateSut();
    int executions = 0;
    sut.AddExecution(new Execution(_ => executions++, (_, _) => { executions++; return Task.CompletedTask; }), false);

    // Act
    sut.Save();
    sut.Save();

    // Assert
    // The queue is drained before running: a second Save must not rewrite the same documents.
    executions.Should().Be(1);
  }

  [Fact]
  public void Save_ShouldNotReplayOperations_AfterAFailedCommit()
  {
    // Arrange
    _probe.Setup(probe => probe.AreTransactionsSupported()).Returns(false);
    StateManager sut = CreateSut(TransactionMode.Required);
    int executions = 0;
    sut.AddExecution(new Execution(_ => executions++, (_, _) => { executions++; return Task.CompletedTask; }), true);

    // Act
    Action act = sut.Save;
    act.Should().Throw<TransactionsNotSupportedException>();
    sut.Save();

    // Assert
    executions.Should().Be(0);
  }

  [Fact]
  public void Dispose_ShouldReleaseTheSession()
  {
    // Arrange
    StateManager sut = CreateSut();
    _ = sut.Session;

    // Act
    sut.Dispose();

    // Assert
    _session.Verify(session => session.Dispose(), Times.Once);
  }

  [Fact]
  public void Dispose_ShouldAbortAPendingTransaction_WithANonCancellableToken()
  {
    // Arrange
    _session.SetupGet(session => session.IsInTransaction).Returns(true);
    StateManager sut = CreateSut();
    _ = sut.Session;

    // Act
    sut.Dispose();

    // Assert
    // A cancelled token would cancel the abort itself, leaving the transaction open server-side.
    _session.Verify(session => session.AbortTransaction(CancellationToken.None), Times.Once);
    _session.Verify(session => session.Dispose(), Times.Once);
  }

  [Fact]
  public void Dispose_ShouldNotThrow_WhenTheAbortFails()
  {
    // Arrange
    _session.SetupGet(session => session.IsInTransaction).Returns(true);
    _session
      .Setup(session => session.AbortTransaction(It.IsAny<CancellationToken>()))
      .Throws(new MongoClientException("connection lost"));
    StateManager sut = CreateSut();
    _ = sut.Session;

    // Act
    Action act = sut.Dispose;

    // Assert
    // A failing compensation must never mask the error that caused the dispose.
    act.Should().NotThrow();
    _session.Verify(session => session.Dispose(), Times.Once);
  }

  [Fact]
  public void Dispose_ShouldBeIdempotent()
  {
    // Arrange
    StateManager sut = CreateSut();
    _ = sut.Session;

    // Act
    sut.Dispose();
    sut.Dispose();

    // Assert
    // The container may track the same instance more than once through the factory registrations.
    _session.Verify(session => session.Dispose(), Times.Once);
  }

  [Fact]
  public void AddExecution_ShouldThrow_WhenTheStateManagerIsDisposed()
  {
    // Arrange
    StateManager sut = CreateSut();
    sut.Dispose();

    // Act
    Action act = () => sut.AddExecution(Recording(new List<IClientSessionHandle>()), false);

    // Assert
    act.Should().Throw<ObjectDisposedException>();
  }
}
