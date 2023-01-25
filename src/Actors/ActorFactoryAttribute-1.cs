namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Interface)]
public class ActorFactoryAttribute<TActor> : Attribute
  where TActor : class
{
  public Type ActorType => typeof(TActor);
}
