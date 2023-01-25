namespace CodeArchitects.Platform.Data;

internal interface IStateManager
{
  Task SaveAsync(CancellationToken cancellationToken);
}
