using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Specifies that a class is an actor.
/// </summary>
/// <typeparam name="TInterface">The type of the interface that clients can use to interact with the actor.</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ActorAttribute<TInterface> : Attribute, IActorAttribute
  where TInterface : class
{
  /// <summary>
  /// The type of the interface that clients can use to interact with the actor.
  /// </summary>
  public Type? InterfaceType => typeof(TInterface);
}
