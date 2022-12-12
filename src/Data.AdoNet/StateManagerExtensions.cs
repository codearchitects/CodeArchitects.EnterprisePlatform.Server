using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal static class StateManagerExtensions
{
  public static Task ExecuteAsync<TDbConnection>(this IStateManager<TDbConnection> stateManager, Execution<TDbConnection, DbTransaction> execution, CancellationToken cancellationToken)
    where TDbConnection : DbConnection
  {
    stateManager.AddExecution(execution);
    return stateManager.SaveAsync(cancellationToken);
  }
}
