namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Specifies that a method does not depend on or modify the actor's state, and can be executed without interacting with the actor's state store.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class StatelessAttribute : Attribute
{
}
