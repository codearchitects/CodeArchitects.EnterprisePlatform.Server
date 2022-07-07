namespace CodeArchitects.Platform.Messaging.AspNetCore;

/// <summary>
/// Thrown when a message cannot be deserialized to the configured type.
/// </summary>
internal class InvalidMessageTypeException : Exception
{
  /// <summary>
  /// Creates a new <see cref="InvalidMessageTypeException"/> instance.
  /// </summary>
  /// <param name="messageType">The configured type.</param>
  public InvalidMessageTypeException(Type messageType)
  {
    MessageType = messageType;
  }

  /// <summary>
  /// The configured type.
  /// </summary>
  public Type MessageType { get; }
}
