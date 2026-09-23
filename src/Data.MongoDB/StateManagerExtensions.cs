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
    // Inside a unit of work the operation is only queued and this token never reaches the
    // driver: an operation already cancelled by its caller must not be committed later.
    cancellationToken.ThrowIfCancellationRequested();

    stateManager.AddExecution(execution, requiresTransaction);

    return stateManager.SaveAsync(cancellationToken);
  }
}
