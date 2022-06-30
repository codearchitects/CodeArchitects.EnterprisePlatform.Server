namespace CodeArchitects.Platform.Messaging.Descriptors;

/// <summary>
/// Describes a message.
/// </summary>
internal interface IMessageDescriptor
{
  /// <summary>
  /// The type of the message.
  /// </summary>
  Type Type { get; }

  /// <summary>
  /// The name of the message.
  /// </summary>
  string Name { get; }
}
