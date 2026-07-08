using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Indicates that an interface is to be used for constructing actor proxies.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class ActorFactoryAttribute : Attribute, IActorFactoryAttribute
{
  /// <summary>
  /// Creates a new <see cref="ActorFactoryAttribute"/> instance.
  /// </summary>
  /// <param name="actorType">The actor type the interface creates proxies for.</param>
  public ActorFactoryAttribute(Type actorType)
  {
    ActorType = actorType;
  }

  /// <summary>
  /// The actor type the interface creates proxies for.
  /// </summary>
  public Type ActorType { get; }
}
