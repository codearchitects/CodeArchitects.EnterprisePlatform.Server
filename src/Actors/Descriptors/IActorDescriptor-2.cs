using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorDescriptor<TActor, TState> : IActorDescriptor
  where TActor : class
  where TState : ActorState
{
  new IStateDescriptor<TState> State { get; }

  new IActorIdDescriptor<TState> Id { get; }

  new IImplementationDescriptor<TActor, TState> BaseImplementation { get; }

  new IImplementationDescriptor<TActor, TState> DefaultImplementation { get; }

  new IReadOnlyCollection<IImplementationDescriptor<TActor, TState>> Implementations { get; }

  TActor CreateInstance(int implementationId, IServiceProvider services, TState state, IActorContext<TActor> context);

  new IImplementationDescriptor<TActor, TState> GetImplementation(Type implementationType);

  void UpdateState(TActor actor, TState state);
}
