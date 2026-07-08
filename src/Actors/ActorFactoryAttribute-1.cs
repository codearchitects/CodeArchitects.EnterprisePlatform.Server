using CodeArchitects.Platform.Actors.Metadata.Reflection;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Indicates that an interface is to be used for constructing actor proxies.
/// </summary>
/// <typeparam name="TActor">The actor type the interface creates proxies for.</typeparam>
[AttributeUsage(AttributeTargets.Interface)]
public class ActorFactoryAttribute<TActor> : Attribute, IActorFactoryAttribute
  where TActor : class
{
  /// <summary>
  /// The actor type the interface creates proxies for.
  /// </summary>
  public Type ActorType => typeof(TActor);
}
