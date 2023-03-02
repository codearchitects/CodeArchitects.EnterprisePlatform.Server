using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Indicates that a class is an implementation for an actor.
/// </summary>
/// <remarks>
/// The decorated class must inherit (directly or indirectly) from the specified actor type.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class ActorImplementationAttribute : Attribute, IActorImplementationAttribute
{
  /// <summary>
  /// Creates a new <see cref="ActorImplementationAttribute"/> instance.
  /// </summary>
  /// <param name="actorType">The actor type which the decorated class is an implementation of.</param>
  public ActorImplementationAttribute(Type actorType)
  {
    ActorType = actorType;
  }

  /// <summary>
  /// The actor type which the decorated class is an implementation of.
  /// </summary>
  public Type ActorType { get; }

  /// <summary>
  /// Indicates whether the implementation is the default one.
  /// </summary>
  public bool IsDefault { get; set; }
}
