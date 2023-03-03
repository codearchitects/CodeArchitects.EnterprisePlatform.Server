using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Class)]
public class ActorIdTypeAttribute : Attribute, IActorIdTypeAttribute
{
  public ActorIdTypeAttribute(Type idType)
  {
    IdType = idType;
  }

  public Type IdType { get; }
}
