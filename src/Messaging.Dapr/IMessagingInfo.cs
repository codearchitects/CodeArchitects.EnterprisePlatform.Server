namespace CodeArchitects.Platform.Messaging.Dapr;

/// <summary>
/// Provides information about messaging.
/// </summary>
internal interface IMessagingInfo
{
  /// <summary>
  /// Returns the name of a message.
  /// </summary>
  /// <param name="messageType">The message type.</param>
  /// <returns>The name of the message.</returns>
  string GetMessageName(Type messageType);

  /// <summary>
  /// Checks whether a bus is known.
  /// </summary>
  /// <param name="busName">The name of the bus.</param>
  /// <returns><see langword="true"/> if the bus is known, <see langword="false"/> otherwise.</returns>
  bool IsBusKnown(string busName);

  /// <summary>
  /// Gets the default bus.
  /// </summary>
  /// <returns>The name of the default bus.</returns>
  string? GetDefaultBus();
}
