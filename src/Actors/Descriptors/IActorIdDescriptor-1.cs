using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorIdDescriptor<TState> : IActorIdDescriptor
  where TState : ActorState
{
  void SetId(TState state, string id);
}
