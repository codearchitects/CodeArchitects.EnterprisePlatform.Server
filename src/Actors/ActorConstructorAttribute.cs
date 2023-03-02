namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Specifies that a constructor is to be used for creating instances of the actor when managed by the host.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
public sealed class ActorConstructorAttribute : Attribute
{
}
