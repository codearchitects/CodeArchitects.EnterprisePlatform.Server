namespace CodeArchitects.Platform.Data;

internal interface IStateManager
{
  void Save();
  Task SaveAsync(CancellationToken cancellationToken);
}
