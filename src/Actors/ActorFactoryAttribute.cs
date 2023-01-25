namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class ActorFactoryAttribute : Attribute
{
  public ActorFactoryAttribute(Type actorType)
  {
    ActorType = actorType;
  }

  public Type ActorType { get; }
}
