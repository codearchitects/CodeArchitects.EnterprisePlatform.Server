using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal delegate TActor ImplementationFactory<TActor, TState>(IServiceProvider services, TState state, IActorContext<TActor> context)
  where TActor : class
  where TState : ActorState;
