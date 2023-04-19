using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Specifies the type to use for the actor id.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ActorIdTypeAttribute : Attribute, IActorIdTypeAttribute
{
  /// <summary>
  /// Creates a new instance of <see cref="ActorIdTypeAttribute"/>.
  /// </summary>
  /// <param name="idType">The actor id type.</param>
  public ActorIdTypeAttribute(Type idType)
  {
    IdType = idType;
  }

  /// <summary>
  /// The actor id type.
  /// </summary>
  public Type IdType { get; }
}
