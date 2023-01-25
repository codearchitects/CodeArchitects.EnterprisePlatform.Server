using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal static class StateManagerExtensions
{
  public static Task ExecuteAsync<TDbConnection>(this IStateManager<TDbConnection> stateManager, Execution<TDbConnection, DbTransaction> execution, bool startTransaction, CancellationToken cancellationToken)
    where TDbConnection : DbConnection
  {
    stateManager.AddExecution(execution, startTransaction);
    return stateManager.SaveAsync(cancellationToken);
  }
}
