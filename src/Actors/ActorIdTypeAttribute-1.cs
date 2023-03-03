using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Class)]
public class ActorIdTypeAttribute<TActorId> : Attribute, IActorIdTypeAttribute
{
  public Type IdType => typeof(TActorId);
}
