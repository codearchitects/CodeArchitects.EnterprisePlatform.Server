using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Specifies the type to use for the actor id.
/// </summary>
/// <typeparam name="TActorId">The actor id type.</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public class ActorIdTypeAttribute<TActorId> : Attribute, IActorIdTypeAttribute
{
  /// <summary>
  /// The actor id type.
  /// </summary>
  public Type IdType => typeof(TActorId);
}
