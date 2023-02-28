using CodeArchitects.Platform.Actors.Infrastructure;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal abstract class StateActorIdDescriptor<TState> : IActorIdDescriptor<TState>
  where TState : ActorState
{
  private readonly Action<TState, string> _setId;

  protected StateActorIdDescriptor(int stateIndex, Action<TState, string> setId)
  {
    StateIndex = stateIndex;
    _setId = setId;
  }

  public abstract Type Type { get; }

  public abstract MethodInfo? GetActorIdMethod { get; }

  public bool HasIdSource => true;

  public int StateIndex { get; }

  public void SetId(TState state, string id)
  {
    _setId(state, id);
  }
}
