using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IStateDescriptor<TState> : IStateDescriptor
  where TState : ActorState
{
  new TState? DefaultValue { get; }
}
