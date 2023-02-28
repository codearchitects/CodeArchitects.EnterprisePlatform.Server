using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class ActorFactoryAttribute : Attribute, IActorFactoryAttribute
{
  public ActorFactoryAttribute(Type actorType)
  {
    ActorType = actorType;
  }

  public Type ActorType { get; }
}
