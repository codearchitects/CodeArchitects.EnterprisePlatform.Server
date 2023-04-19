namespace CodeArchitects.Platform.Actors.Bindings;

internal interface IActorBinding<TActor>
  where TActor : class
{
  void VerifyPreCondition(TActor actor);

  Task ExecuteAsync(TActor actor, CancellationToken cancellationToken);
}
