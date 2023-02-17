using CodeArchitects.Platform.Actors.Descriptors.Reflection;

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
