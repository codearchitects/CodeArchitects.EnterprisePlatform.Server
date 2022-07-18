namespace CodeArchitects.Platform.Messaging;

/// <summary>
/// Attribute that prevent a message handler method to be interpreted as a handler method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class DoesNotHandleAttribute : Attribute
{
}
