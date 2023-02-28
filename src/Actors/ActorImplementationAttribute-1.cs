using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Class)]
public class ActorImplementationAttribute<TActor> : Attribute, IActorImplementationAttribute
  where TActor : class
{
  public Type ActorType => typeof(TActor);

  public bool IsDefault { get; set; }
}
