using CodeArchitects.Platform.Data.MongoDB.Transactions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Owns the MongoDB session of the scope and turns the pending writes into a single commit.
/// </summary>
internal sealed class StateManager : Data.StateManager, IStateManager, IDisposable, IAsyncDisposable
{
  private readonly IMongoClient _client;
  private readonly ITransactionCapabilityProbe _probe;
  private readonly MongoDBOptions _options;
  private readonly ILogger _logger;
  private readonly List<Execution> _executions;

  private IClientSessionHandle? _session;
  private bool _requiresTransaction;
  private bool _isDisposed;

  public StateManager(
    IMongoClient client,
    ITransactionCapabilityProbe probe,
    MongoDBOptions options,
    ILogger<StateManager> logger)
  {
    _client = client;
    _probe = probe;
    _options = options;
    _logger = logger;
    _executions = new(2);
  }

  public IClientSessionHandle Session
  {
    get
    {
      EnsureNotDisposed();

      // Lazy: a scope that never touches MongoDB must not pay for a session.
      return _session ??= _client.StartSession();
    }
  }

  public void AddExecution(Execution execution, bool requiresTransaction)
  {
    EnsureNotDisposed();

    _requiresTransaction |= requiresTransaction;
    _executions.Add(execution);
  }

  protected override void SaveCore()
  {
    EnsureNotDisposed();

    if (_executions.Count == 0)
      return;

    (List<Execution> batch, bool requiresTransaction) = Drain();
    IClientSessionHandle session = Session;

    if (!ShouldUseTransaction(batch.Count, requiresTransaction))
    {
      foreach (Execution execution in batch)
      {
        execution.Sync(session);
      }
      return;
    }

    session.WithTransaction(
      (transactionSession, _) =>
      {
        foreach (Execution execution in batch)
        {
          execution.Sync(transactionSession);
        }
        return true;
      },
      _options.TransactionOptions,
      CancellationToken.None);
  }

  protected override async Task SaveCoreAsync(CancellationToken cancellationToken)
  {
    EnsureNotDisposed();

    if (_executions.Count == 0)
      return;

    (List<Execution> batch, bool requiresTransaction) = Drain();
    IClientSessionHandle session = Session;

    if (!ShouldUseTransaction(batch.Count, requiresTransaction))
    {
      foreach (Execution execution in batch)
      {
        await execution.Async(session, cancellationToken);
      }
      return;
    }

    // WithTransactionAsync implements the retry procedure the MongoDB driver specification
    // prescribes for TransientTransactionError and UnknownTransactionCommitResult, and aborts
    // the transaction when the callback throws.
    await session.WithTransactionAsync(
      async (transactionSession, token) =>
      {
        foreach (Execution execution in batch)
        {
          await execution.Async(transactionSession, token);
        }
        return true;
      },
      _options.TransactionOptions,
      cancellationToken);
  }

  /// <summary>
  /// Takes the pending writes and clears the queue before running them, so a failed commit
  /// cannot be replayed by a later Save.
  /// </summary>
  private (List<Execution> Batch, bool RequiresTransaction) Drain()
  {
    List<Execution> batch = new(_executions);
    bool requiresTransaction = _requiresTransaction;

    _executions.Clear();
    _requiresTransaction = false;

    return (batch, requiresTransaction);
  }

  protected override void DiscardPending()
  {
    _executions.Clear();
    _requiresTransaction = false;
  }

  private bool ShouldUseTransaction(int executionCount, bool requiresTransaction)
  {
    if (_options.TransactionMode == TransactionMode.Disabled)
      return false;

    // A write on a single document is already atomic in MongoDB: wrapping it in a transaction
    // would add two round-trips and transactional locks for no additional guarantee.
    if (executionCount <= 1 && !requiresTransaction)
      return false;

    if (_probe.AreTransactionsSupported())
      return true;

    if (_options.TransactionMode == TransactionMode.Required)
      throw new TransactionsNotSupportedException();

    _logger.LogWarning(
      "Transactions are not supported by the current topology: {Count} operations will run without atomicity.",
      executionCount);

    return false;
  }

  public void Dispose()
  {
    if (_isDisposed)
      return;

    _isDisposed = true;
    _executions.Clear();

    try
    {
      if (_session is { IsInTransaction: true })
      {
        // CancellationToken.None on purpose: see DisposeAsync.
        _session.AbortTransaction(CancellationToken.None);
      }
    }
    catch (Exception exception)
    {
      // Never let a compensating action mask the failure that caused the dispose.
      _logger.LogWarning(exception, "Failed to abort the MongoDB transaction while disposing the state manager.");
    }
    finally
    {
      _session?.Dispose();
      _session = null;
    }
  }

  public async ValueTask DisposeAsync()
  {
    if (_isDisposed)
      return;

    _isDisposed = true;
    _executions.Clear();

    try
    {
      if (_session is { IsInTransaction: true })
      {
        // A cancelled token would cancel the abort itself, leaving the transaction open on the
        // server until transactionLifetimeLimitSeconds expires, holding its locks.
        await _session.AbortTransactionAsync(CancellationToken.None);
      }
    }
    catch (Exception exception)
    {
      _logger.LogWarning(exception, "Failed to abort the MongoDB transaction while disposing the state manager.");
    }
    finally
    {
      _session?.Dispose();
      _session = null;
    }
  }

  private void EnsureNotDisposed()
  {
    if (_isDisposed)
      throw new ObjectDisposedException(nameof(StateManager));
  }
}
