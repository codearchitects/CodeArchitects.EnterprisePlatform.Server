using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IActorIdDescriptor<TState> : IActorIdDescriptor
  where TState : ActorState
{
  void SetId(TState state, string id);
}
