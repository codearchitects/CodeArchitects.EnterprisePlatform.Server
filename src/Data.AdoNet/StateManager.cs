using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class StateManager<TDbConnection> : StateManager, IStateManager<TDbConnection>
  where TDbConnection : DbConnection
{
  private readonly List<Execution<TDbConnection>> _executions;
  
  public StateManager(TDbConnection connection)
  {
    Connection = connection;
    _executions = new(2);
  }

  public TDbConnection Connection { get; }

  public void AddExecution(Execution<TDbConnection> execution)
  {
    _executions.Add(execution);
  }

  protected override async Task SaveCoreAsync(CancellationToken cancellationToken)
  {
    // TODO: Open transaction if _executions.Count > 1

    await Connection.OpenAsync(cancellationToken);
    try
    {
      foreach (Execution<TDbConnection> execution in _executions)
      {
        await execution(Connection, cancellationToken);
      }
    }
    finally
    {
      _executions.Clear();
      Connection.Close();
    }
  }
}
