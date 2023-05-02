using CodeArchitects.Platform.Data.AdoNet.Features.Concurrency;
using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class StateManager<TDbConnection> : StateManager, IStateManager<TDbConnection>, IDisposable
  where TDbConnection : DbConnection
{
  private readonly IConnectionFactory<TDbConnection> _connectionFactory;
  private readonly IConcurrencyContext _concurrencyContext;
  private readonly List<Execution<TDbConnection, DbTransaction>> _executions;
  private bool _startTransaction;
  private TDbConnection? _connection;
  private bool _isDisposed;

  public StateManager(IConnectionFactory<TDbConnection> connectionFactory, IConcurrencyContext concurrencyContext)
  {
    _connectionFactory = connectionFactory;
    _concurrencyContext = concurrencyContext;
    _executions = new(2);
  }

  public TDbConnection Connection => _connection ??= _connectionFactory.CreateConnection();

  public void AddExecution(Execution<TDbConnection, DbTransaction> execution, bool startTransaction)
  {
    EnsureNotDisposed();

    _startTransaction |= startTransaction;
    _executions.Add(execution);
  }

  protected override async Task SaveCoreAsync(CancellationToken cancellationToken)
  {
    EnsureNotDisposed();

    await Connection.OpenAsync(cancellationToken);

    DbTransaction? transaction = _startTransaction || _executions.Count > 1
      ? await Connection.BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken)
      : null;

    try
    {
      foreach (var execution in _executions)
      {
        await execution(Connection, transaction, cancellationToken);
      }
      if (transaction is not null)
      {
        await transaction.CommitAsync(cancellationToken);
      }

      _concurrencyContext.Accept();
    }
    catch when (transaction is not null)
    {
      await transaction.RollbackAsync(cancellationToken);
      throw;
    }
    finally
    {
      if (transaction is not null)
      {
        await transaction.DisposeAsync();
      }
      await Connection.CloseAsync();
      _startTransaction = false;
      _executions.Clear();
      _concurrencyContext.Clear();
    }
  }

  protected override void SaveCore()
  {
    EnsureNotDisposed();

    Connection.Open();

    DbTransaction? transaction = _startTransaction || _executions.Count > 1
      ? Connection.BeginTransaction(IsolationLevel.Unspecified)
      : null;

    try
    {
      foreach (var execution in _executions)
      {
        execution(Connection, transaction, CancellationToken.None).GetAwaiter().GetResult();
      }

      transaction?.Commit();
    }
    catch when (transaction is not null)
    {
      transaction.Rollback();
      throw;
    }
    finally
    {
      _startTransaction = false;
      transaction?.Dispose();
      _executions.Clear();
      Connection.Close();
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (_isDisposed)
      return;

    if (disposing)
    {
      _connection?.Dispose();
    }

    _isDisposed = true;
  }

  private void EnsureNotDisposed()
  {
    if (_isDisposed)
      throw new ObjectDisposedException(nameof(StateManager<TDbConnection>));
  }
}
