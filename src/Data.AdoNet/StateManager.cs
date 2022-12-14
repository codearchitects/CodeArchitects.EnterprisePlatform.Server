using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class StateManager<TDbConnection> : StateManager, IStateManager<TDbConnection>
  where TDbConnection : DbConnection
{
  private bool _startTransaction;
  private readonly List<Execution<TDbConnection, DbTransaction>> _executions;
  
  public StateManager(TDbConnection connection)
  {
    Connection = connection;
    _executions = new(2);
  }

  public TDbConnection Connection { get; }

  public void AddExecution(Execution<TDbConnection, DbTransaction> execution, bool startTransaction)
  {
    _startTransaction |= startTransaction;
    _executions.Add(execution);
  }

  protected override async Task SaveCoreAsync(CancellationToken cancellationToken)
  {
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
    }
    catch when (transaction is not null)
    {
      await transaction.RollbackAsync(cancellationToken);
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
}
