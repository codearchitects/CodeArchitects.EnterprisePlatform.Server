namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Specifies that an actor is virtual, meaning it does not need to be explicitly initialized.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VirtualAttribute : Attribute
{
}
