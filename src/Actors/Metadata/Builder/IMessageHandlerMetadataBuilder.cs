namespace CodeArchitects.Platform.Actors.Metadata.Builder;

/// <summary>
/// A builder that can be used to configure a message handler method.
/// </summary>
public interface IMessageHandlerMetadataBuilder
{
  /// <summary>
  /// Specifies the name of the bus to subscribe to.
  /// </summary>
  /// <param name="bus">The name of the bus.</param>
  /// <returns>The same builder.</returns>
  IMessageHandlerMetadataBuilder WithBus(string bus);

  /// <summary>
  /// Specifies the name of the topic to subscribe to.
  /// </summary>
  /// <param name="topic">The name of the topic.</param>
  /// <returns>The same builder.</returns>
  IMessageHandlerMetadataBuilder WithTopic(string topic);
}
