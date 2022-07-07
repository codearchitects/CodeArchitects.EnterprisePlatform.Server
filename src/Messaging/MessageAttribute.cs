namespace CodeArchitects.Platform.Messaging;

/// <summary>
/// Attribute used to mark messages.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class MessageAttribute : Attribute
{
  /// <summary>
  /// Creates a new <see cref="MessageAttribute"/> instance specifying without a message name.
  /// </summary>
  public MessageAttribute()
  {
  }

  /// <summary>
  /// Creates a new <see cref="MessageAttribute"/> instance specifying a message name.
  /// </summary>
  /// <param name="messageName">The name of the message.</param>
  public MessageAttribute(string messageName)
  {
    MessageName = messageName;
  }

  /// <summary>
  /// The name of the message.
  /// </summary>
  /// <remarks>
  /// If null, the name of the CLR type of the message will be used.
  /// </remarks>
  public string? MessageName { get; }
}
