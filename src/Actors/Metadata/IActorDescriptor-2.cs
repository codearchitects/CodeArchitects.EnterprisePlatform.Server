using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IActorDescriptor<TActor, TState> : IActorDescriptor
  where TActor : class
  where TState : ActorState
{
  new IStateDescriptor<TState> State { get; }

  new IActorIdDescriptor<TState> Id { get; }

  new IImplementationDescriptor<TActor, TState> BaseImplementation { get; }

  new IImplementationDescriptor<TActor, TState> DefaultImplementation { get; }

  new IReadOnlyCollection<IImplementationDescriptor<TActor, TState>> Implementations { get; }

  new IImplementationDescriptor<TActor, TState> GetImplementation(Type implementationType);

  TActor CreateInstance(int implementationId, IServiceProvider services, TState state, IActorContext<TActor> context);

  void UpdateState(TActor actor, TState state);
}
