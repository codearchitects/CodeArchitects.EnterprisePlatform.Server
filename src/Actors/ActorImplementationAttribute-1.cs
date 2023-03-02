using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Indicates that a class is an implementation for an actor.
/// </summary>
/// <remarks>
/// The decorated class must inherit (directly or indirectly) from the specified actor type.
/// </remarks>
/// <typeparam name="TActor">The actor type which the decorated class is an implementation of.</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public class ActorImplementationAttribute<TActor> : Attribute, IActorImplementationAttribute
  where TActor : class
{
  /// <summary>
  /// The actor type which the decorated class is an implementation of.
  /// </summary>
  public Type ActorType => typeof(TActor);

  /// <summary>
  /// Indicates whether the implementation is the default one.
  /// </summary>
  public bool IsDefault { get; set; }
}
