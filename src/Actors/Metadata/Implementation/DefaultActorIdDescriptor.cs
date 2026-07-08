using CodeArchitects.Platform.Actors.Infrastructure;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class DefaultActorIdDescriptor<TState> : IActorIdDescriptor<TState>
  where TState : ActorState
{
  public DefaultActorIdDescriptor(Type type)
  {
    Type = type;
  }

  public Type Type { get; }

  public bool HasIdSource => false;

  public int StateIndex => -1;

  public MethodInfo? GetActorIdMethod => null;

  public void SetId(TState state, string id)
  {
    // The state does not contain any id field to set
  }
}
