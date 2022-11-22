using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal static class StateManagerExtensions
{
  public static Task ExecuteAsync<TDbConnection>(this IStateManager<TDbConnection> stateManager, Execution<TDbConnection> execution, CancellationToken cancellationToken = default)
    where TDbConnection : DbConnection
  {
    stateManager.AddExecution(execution);
    return stateManager.SaveAsync(cancellationToken);
  }
}
