using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorDescriptor<TActor, TState> : IActorDescriptor
  where TActor : class
  where TState : ActorState
{
  new IStateDescriptor<TState> State { get; }

  TActor CreateInstance(int implementationId, IServiceProvider services, TState state, IActorContext<TActor> context);

  void UpdateState(TActor actor, TState state);
}
