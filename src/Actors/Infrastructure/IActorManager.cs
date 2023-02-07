namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorManager<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  int GetImplementationId(Type implementationType);
}
