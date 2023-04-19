using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Indicates that a class is an actor.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ActorAttribute : Attribute, IActorAttribute
{
  /// <summary>
  /// The type of the interface that clients can use to interact with the actor.
  /// </summary>
  public Type? InterfaceType { get; set; }
}
