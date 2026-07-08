using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class StateActorIdDescriptor<TState> : IActorIdDescriptor<TState>
  where TState : ActorState
{
  private readonly Action<TState, string> _setId;

  public StateActorIdDescriptor(Type type, int stateIndex, Action<TState, string> setId)
  {
    Type = type;
    StateIndex = stateIndex;
    _setId = setId;
  }

  public Type Type { get; }

  public bool HasIdSource => true;

  public int StateIndex { get; }

  public void SetId(TState state, string id)
  {
    _setId(state, id);
  }
}
