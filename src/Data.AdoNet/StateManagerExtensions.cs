using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal static class StateManagerExtensions
{
  public static void Execute<TDbConnection>(this IStateManager<TDbConnection> stateManager, Execution<TDbConnection, DbTransaction> execution, bool startTransaction)
    where TDbConnection : DbConnection
  {
    stateManager.AddExecution(execution, startTransaction);
    stateManager.Save();
  }

  public static Task ExecuteAsync<TDbConnection>(this IStateManager<TDbConnection> stateManager, Execution<TDbConnection, DbTransaction> execution, bool startTransaction, CancellationToken cancellationToken)
    where TDbConnection : DbConnection
  {
    stateManager.AddExecution(execution, startTransaction);
    return stateManager.SaveAsync(cancellationToken);
  }
}
