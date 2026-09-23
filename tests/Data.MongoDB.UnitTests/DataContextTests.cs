using CodeArchitects.Platform.Data.MongoDB.Collections;
using CodeArchitects.Platform.Data.MongoDB.Filters;
using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using MongoDB.Driver;
using System.Data;

namespace CodeArchitects.Platform.Data.MongoDB;

public class DataContextTests
{
  private readonly Mock<IMongoCollection<DiscoverableEntity>> _collection = new();
  private readonly ImmediateStateManager _stateManager = new(Mock.Of<IClientSessionHandle>());
  private readonly DataContext _sut;

  public DataContextTests()
  {
    Mock<ICollectionProvider> collections = new();
    collections
      .Setup(provider => provider.GetCollection<DiscoverableEntity>())
      .Returns(_collection.Object);

    _sut = new DataContext(
      new FilterProvider(),
      collections.Object,
      _stateManager,
      new DataModelBuilder().AddEntity(typeof(DiscoverableEntity)).Build(),
      Mock.Of<IMongoDatabase>());
  }

  private static Task Run(bool async, Action sync, Func<Task> asyncOperation)
  {
    if (async)
      return asyncOperation();

    sync();
    return Task.CompletedTask;
  }

  private static DiscoverableEntity Entity() => new() { Id = Guid.NewGuid() };

  #region Driver results

  private void SetupReplace(ReplaceOneResult result)
  {
    _collection
      .Setup(collection => collection.ReplaceOne(
        It.IsAny<IClientSessionHandle>(), It.IsAny<FilterDefinition<DiscoverableEntity>>(),
        It.IsAny<DiscoverableEntity>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
      .Returns(result);
    _collection
      .Setup(collection => collection.ReplaceOneAsync(
        It.IsAny<IClientSessionHandle>(), It.IsAny<FilterDefinition<DiscoverableEntity>>(),
        It.IsAny<DiscoverableEntity>(), It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(result);
  }

  private void SetupBulkWrite(BulkWriteResult<DiscoverableEntity> result)
  {
    _collection
      .Setup(collection => collection.BulkWrite(
        It.IsAny<IClientSessionHandle>(), It.IsAny<IEnumerable<WriteModel<DiscoverableEntity>>>(),
        It.IsAny<BulkWriteOptions>(), It.IsAny<CancellationToken>()))
      .Returns(result);
    _collection
      .Setup(collection => collection.BulkWriteAsync(
        It.IsAny<IClientSessionHandle>(), It.IsAny<IEnumerable<WriteModel<DiscoverableEntity>>>(),
        It.IsAny<BulkWriteOptions>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(result);
  }

  private void SetupDelete(DeleteResult result)
  {
    _collection
      .Setup(collection => collection.DeleteOne(
        It.IsAny<IClientSessionHandle>(), It.IsAny<FilterDefinition<DiscoverableEntity>>(),
        It.IsAny<DeleteOptions>(), It.IsAny<CancellationToken>()))
      .Returns(result);
    _collection
      .Setup(collection => collection.DeleteOneAsync(
        It.IsAny<IClientSessionHandle>(), It.IsAny<FilterDefinition<DiscoverableEntity>>(),
        It.IsAny<DeleteOptions>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(result);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Update_ShouldSucceed_WhenTheDocumentMatchedButNothingChanged(bool async)
  {
    // Arrange
    SetupReplace(new ReplaceOneResult.Acknowledged(matchedCount: 1, modifiedCount: 0, upsertedId: null));
    DiscoverableEntity entity = Entity();

    // Act
    Func<Task> act = () => Run(async,
      () => _sut.Update<DiscoverableEntity, Guid>(entity),
      () => _sut.UpdateAsync<DiscoverableEntity, Guid>(entity));

    // Assert
    await act.Should().NotThrowAsync();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Update_ShouldThrow_WhenNoDocumentMatched(bool async)
  {
    // Arrange
    SetupReplace(new ReplaceOneResult.Acknowledged(matchedCount: 0, modifiedCount: 0, upsertedId: null));
    DiscoverableEntity entity = Entity();

    // Act
    Func<Task> act = () => Run(async,
      () => _sut.Update<DiscoverableEntity, Guid>(entity),
      () => _sut.UpdateAsync<DiscoverableEntity, Guid>(entity));

    // Assert
    await act.Should().ThrowAsync<DBConcurrencyException>().WithMessage($"*{entity.Id}*");
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Update_ShouldNotReportAConflict_WhenTheWriteIsUnacknowledged(bool async)
  {
    // Arrange
    SetupReplace(ReplaceOneResult.Unacknowledged.Instance);
    DiscoverableEntity entity = Entity();

    // Act
    Func<Task> act = () => Run(async,
      () => _sut.Update<DiscoverableEntity, Guid>(entity),
      () => _sut.UpdateAsync<DiscoverableEntity, Guid>(entity));

    // Assert
    // With w:0 the server reports no counts: a missing document cannot be told apart from a
    // successful write, so the outcome must not be reported as a concurrency failure.
    await act.Should().NotThrowAsync();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Upsert_ShouldNotReportAConflict_WhenTheWriteIsUnacknowledged(bool async)
  {
    // Arrange
    SetupReplace(ReplaceOneResult.Unacknowledged.Instance);
    DiscoverableEntity entity = Entity();

    // Act
    Func<Task> act = () => Run(async,
      () => _sut.Upsert<DiscoverableEntity, Guid>(entity),
      () => _sut.UpsertAsync<DiscoverableEntity, Guid>(entity));

    // Assert
    await act.Should().NotThrowAsync();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task UpdateMany_ShouldNotReportAConflict_WhenTheWriteIsUnacknowledged(bool async)
  {
    // Arrange
    DiscoverableEntity[] entities = [Entity(), Entity()];
    SetupBulkWrite(new BulkWriteResult<DiscoverableEntity>.Unacknowledged(
      entities.Length,
      [.. entities.Select(entity => new InsertOneModel<DiscoverableEntity>(entity))]));

    // Act
    Func<Task> act = () => Run(async,
      () => _sut.UpdateMany<DiscoverableEntity, Guid>(entities),
      () => _sut.UpdateManyAsync<DiscoverableEntity, Guid>(entities));

    // Assert
    // Reading MatchedCount of an unacknowledged result throws InvalidOperationException.
    await act.Should().NotThrowAsync();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Remove_ShouldNotReportAConflict_WhenTheWriteIsUnacknowledged(bool async)
  {
    // Arrange
    SetupDelete(DeleteResult.Unacknowledged.Instance);
    Guid key = Guid.NewGuid();

    // Act
    Func<Task> act = () => Run(async,
      () => _sut.Remove<DiscoverableEntity, Guid>(key),
      () => _sut.RemoveAsync<DiscoverableEntity, Guid>(key));

    // Assert
    await act.Should().NotThrowAsync();
  }

  #endregion

  #region Multiple operations

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task InsertMany_ShouldDoNothing_WhenTheSequenceIsEmpty(bool async)
  {
    // Act
    await Run(async,
      () => _sut.InsertMany<DiscoverableEntity, Guid>([]),
      () => _sut.InsertManyAsync<DiscoverableEntity, Guid>([]));

    // Assert
    // Nothing queued means no round-trip and no transaction: the driver would reject an empty
    // batch, and a standalone server would reject the transaction.
    _stateManager.QueuedCount.Should().Be(0);
    _collection.VerifyNoOtherCalls();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task UpdateMany_ShouldDoNothing_WhenTheSequenceIsEmpty(bool async)
  {
    // Act
    await Run(async,
      () => _sut.UpdateMany<DiscoverableEntity, Guid>([]),
      () => _sut.UpdateManyAsync<DiscoverableEntity, Guid>([]));

    // Assert
    _stateManager.QueuedCount.Should().Be(0);
    _collection.VerifyNoOtherCalls();
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task InsertMany_ShouldThrow_WhenAnElementIsNull(bool async)
  {
    // Arrange
    DiscoverableEntity[] entities = [Entity(), null!];

    // Act
    Func<Task> act = () => Run(async,
      () => _sut.InsertMany<DiscoverableEntity, Guid>(entities),
      () => _sut.InsertManyAsync<DiscoverableEntity, Guid>(entities));

    // Assert
    await act.Should().ThrowAsync<ArgumentException>().WithMessage("*index 1*");
    _stateManager.QueuedCount.Should().Be(0);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task UpdateMany_ShouldThrow_WhenAnElementIsNull(bool async)
  {
    // Arrange
    DiscoverableEntity[] entities = [null!, Entity()];

    // Act
    Func<Task> act = () => Run(async,
      () => _sut.UpdateMany<DiscoverableEntity, Guid>(entities),
      () => _sut.UpdateManyAsync<DiscoverableEntity, Guid>(entities));

    // Assert
    await act.Should().ThrowAsync<ArgumentException>().WithMessage("*index 0*");
    _stateManager.QueuedCount.Should().Be(0);
  }

  #endregion

  #region Cancellation

  [Fact]
  public async Task InsertAsync_ShouldNotQueueTheOperation_WhenTheTokenIsAlreadyCancelled()
  {
    // Arrange
    using CancellationTokenSource cancellation = new();
    cancellation.Cancel();

    // Act
    Func<Task> act = () => _sut.InsertAsync<DiscoverableEntity, Guid>(Entity(), cancellation.Token);

    // Assert
    // Inside a unit of work the token never reaches the driver: the operation would otherwise be
    // committed later, although its caller cancelled it.
    await act.Should().ThrowAsync<OperationCanceledException>();
    _stateManager.QueuedCount.Should().Be(0);
  }

  [Fact]
  public async Task UpdateAsync_ShouldPassTheTokenToTheDriver()
  {
    // Arrange
    SetupReplace(new ReplaceOneResult.Acknowledged(matchedCount: 1, modifiedCount: 1, upsertedId: null));
    using CancellationTokenSource cancellation = new();

    // Act
    await _sut.UpdateAsync<DiscoverableEntity, Guid>(Entity(), cancellation.Token);

    // Assert
    _collection.Verify(collection => collection.ReplaceOneAsync(
      It.IsAny<IClientSessionHandle>(), It.IsAny<FilterDefinition<DiscoverableEntity>>(),
      It.IsAny<DiscoverableEntity>(), It.IsAny<ReplaceOptions>(), cancellation.Token));
  }

  #endregion

  /// <summary>
  /// Runs every queued write as soon as it is saved, as the real state manager does outside a
  /// unit of work, and counts what was queued.
  /// </summary>
  private sealed class ImmediateStateManager(IClientSessionHandle session) : IStateManager
  {
    private readonly List<Execution> _pending = [];

    public IClientSessionHandle Session { get; } = session;

    public int QueuedCount { get; private set; }

    public void AddExecution(Execution execution, bool requiresTransaction)
    {
      QueuedCount++;
      _pending.Add(execution);
    }

    public void Save()
    {
      foreach (Execution execution in Drain())
      {
        execution.Sync(Session);
      }
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
      foreach (Execution execution in Drain())
      {
        await execution.Async(Session, cancellationToken);
      }
    }

    private Execution[] Drain()
    {
      Execution[] batch = [.. _pending];
      _pending.Clear();
      return batch;
    }
  }
}
