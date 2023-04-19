using CodeArchitects.Platform.Actors.Infrastructure;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class SourceActorIdDescriptor<TState> : StateActorIdDescriptor<TState>
  where TState : ActorState
{
  public SourceActorIdDescriptor(MethodInfo getActorIdMethod, int stateIndex, Action<TState, string> setId)
    : base(stateIndex, setId)
  {
    GetActorIdMethod = getActorIdMethod;
  }

  public override Type Type => GetActorIdMethod.ReturnType;

  public override MethodInfo GetActorIdMethod { get; }
}
