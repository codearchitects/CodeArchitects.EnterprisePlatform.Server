namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IManagerFactory<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  IActorManager<TActor, TState> Create(IActorHost<TActor> host, TState? state);
  IActorManager<TActor, TState> Create(IActorHost<TActor> host, TState? state, int implementationId);
}
