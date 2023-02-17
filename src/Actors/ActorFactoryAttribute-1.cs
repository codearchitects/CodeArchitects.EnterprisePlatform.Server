using CodeArchitects.Platform.Actors.Descriptors.Reflection;

namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Interface)]
public class ActorFactoryAttribute<TActor> : Attribute, IActorFactoryAttribute
  where TActor : class
{
  public Type ActorType => typeof(TActor);
}
