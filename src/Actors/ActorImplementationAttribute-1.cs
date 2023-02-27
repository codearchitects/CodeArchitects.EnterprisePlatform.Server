using CodeArchitects.Platform.Actors.Descriptors.Reflection;

namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Class)]
public class ActorImplementationAttribute<TActor> : Attribute, IActorImplementationAttribute
  where TActor : class
{
  public Type ActorType => typeof(TActor);

  public bool IsDefault { get; set; }
}
