using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Class)]
public class ActorImplementationAttribute : Attribute, IActorImplementationAttribute
{
  public ActorImplementationAttribute(Type actorType)
  {
    ActorType = actorType;
  }

  public Type ActorType { get; }

  public bool IsDefault { get; set; }
}
