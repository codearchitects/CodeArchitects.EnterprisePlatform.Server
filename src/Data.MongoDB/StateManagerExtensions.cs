namespace CodeArchitects.Platform.Data.MongoDB;

internal static class StateManagerExtensions
{
  public static Task ExecuteAsync(this IStateManager stateManager, Func<CancellationToken, Task> execution, CancellationToken cancellationToken)
  {
    stateManager.AddExecution(execution);
    return stateManager.SaveAsync(cancellationToken);
  }
}
