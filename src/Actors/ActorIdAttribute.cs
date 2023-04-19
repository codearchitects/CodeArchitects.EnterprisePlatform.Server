namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Indicates that a field or property represents the id of an actor.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ActorIdAttribute : Attribute
{
}
