namespace CodeArchitects.Platform.Data.MongoDB;

internal interface IStateManager : Data.IStateManager
{
  void AddExecution(Func<CancellationToken, Task> execution);
}
