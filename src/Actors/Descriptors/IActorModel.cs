using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorModel
{
  IActorDescriptor<TActor, TState> GetDescriptor<TActor, TState>()
    where TActor : class
    where TState : ActorState;
}
