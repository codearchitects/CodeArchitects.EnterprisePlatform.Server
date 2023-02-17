using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IImplementationDescriptor<TActor, TState> : IImplementationDescriptor
  where TActor : class
  where TState : ActorState
{
  TActor CreateInstance(IServiceProvider services, TState state, IActorContext<TActor> context);
}
