namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Specifies that a class is an actor.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ActorAttribute : Attribute
{
  /// <summary>
  /// The type of the interface that clients can use to interact with the actor.
  /// </summary>
  public Type? InterfaceType { get; set; }
}
