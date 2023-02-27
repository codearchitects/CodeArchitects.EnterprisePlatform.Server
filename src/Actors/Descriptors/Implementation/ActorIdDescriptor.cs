using CodeArchitects.Platform.Actors.Infrastructure;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ActorIdDescriptor<TState> : IActorIdDescriptor<TState>
  where TState : ActorState
{
  public ActorIdDescriptor(Type type)
  {
    Type = type;
  }

  public Type Type { get; }

  public bool HasIdSource => false;

  public int StateIndex => throw new InvalidOperationException("The id does not come from the actor's state.");

  public MethodInfo? GetActorIdMethod => throw new InvalidOperationException("The id does not come from the actor's state.");

  public void SetId(TState state, string id)
  {
  }
}
