using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class StateManager<TDbConnection> : StateManager, IStateManager<TDbConnection>
  where TDbConnection : DbConnection
{
  private readonly List<Execution<TDbConnection, DbTransaction>> _executions;
  
  public StateManager(TDbConnection connection)
  {
    Connection = connection;
    _executions = new(2);
  }

  public TDbConnection Connection { get; }

  public void AddExecution(Execution<TDbConnection, DbTransaction> execution)
  {
    _executions.Add(execution);
  }

  protected override async Task SaveCoreAsync(CancellationToken cancellationToken)
  {
    DbTransaction? transaction = _executions.Count > 1
      ? await Connection.BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken)
      : null;

    await Connection.OpenAsync(cancellationToken);
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
      transaction?.Dispose();
      _executions.Clear();
      Connection.Close();
    }
  }
}
