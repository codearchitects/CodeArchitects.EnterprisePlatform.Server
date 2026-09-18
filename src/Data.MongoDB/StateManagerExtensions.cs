namespace CodeArchitects.Platform.Data.MongoDB;

internal static class StateManagerExtensions
{
  public static void Execute(this IStateManager stateManager, Execution execution, bool requiresTransaction)
  {
    stateManager.AddExecution(execution, requiresTransaction);

    // Inside a unit of work Save is a no-op: the write stays queued until the commit.
    stateManager.Save();
  }

  public static Task ExecuteAsync(
    this IStateManager stateManager,
    Execution execution,
    bool requiresTransaction,
    CancellationToken cancellationToken)
  {
    stateManager.AddExecution(execution, requiresTransaction);

    return stateManager.SaveAsync(cancellationToken);
  }
}
