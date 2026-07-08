namespace CodeArchitects.Platform.Actors.Scheduling;

internal abstract class Activity<TActor> : Activity
  where TActor : class
{
  public abstract Task ExecuteAsync(TActor actor, CancellationToken cancellationToken);
}
