using CodeArchitects.Platform.Actors.Infrastructure;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ComponentActorIdDescriptor<TState> : StateActorIdDescriptor<TState>
  where TState : ActorState
{
  public ComponentActorIdDescriptor(Type type, int stateIndex, Action<TState, string> setId)
    : base(stateIndex, setId)
  {
    Type = type;
  }

  public override Type Type { get; }

  public override MethodInfo? GetActorIdMethod => null;
}
