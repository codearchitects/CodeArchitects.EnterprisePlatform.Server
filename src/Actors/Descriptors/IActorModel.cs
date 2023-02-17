using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorModel
{
  IReadOnlyCollection<IActorDescriptor> Actors { get; }

  IActorDescriptor<TActor, TState> GetActor<TActor, TState>()
    where TActor : class
    where TState : ActorState;
}
