namespace CodeArchitects.Platform.Actors.Infrastructure;

internal interface IImplementationFactory<TActor, TState>
  where TActor : class
  where TState : ActorState
{
  TActor Create(IActorHost<TActor, TState> host, TState state);
  TActor Create(IActorHost<TActor, TState> host, TState state, int implementationId);
}
