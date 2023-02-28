using CodeArchitects.Platform.Actors.Infrastructure;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class DefaultActorIdDescriptor<TState> : IActorIdDescriptor<TState>
  where TState : ActorState
{
  public static readonly DefaultActorIdDescriptor<TState> Instance = new();

  private DefaultActorIdDescriptor() { }

  public Type Type => typeof(string);

  public bool HasIdSource => false;

  public int StateIndex => -1;

  public MethodInfo? GetActorIdMethod => null;

  public void SetId(TState state, string id)
  {
  }
}
