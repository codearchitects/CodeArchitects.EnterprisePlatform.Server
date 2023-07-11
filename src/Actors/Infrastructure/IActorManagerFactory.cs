namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IActorManagerFactory<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  IActorManager<TActor> CreateManager(IActorHost<TActor, TState> host);
}
