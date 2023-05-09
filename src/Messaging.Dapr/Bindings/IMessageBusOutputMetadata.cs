using CodeArchitects.Platform.Messaging.Bindings;

namespace CodeArchitects.Platform.Messaging.Dapr.Bindings;

/// <summary>
/// The metadata for the message bus output binding. Specifies the information for publishing the result of a message handler to a bus.
/// </summary>
public interface IMessageBusOutputMetadata : ITypedOutputMetadata
{
  /// <summary>
  /// The name of the bus.
  /// </summary>
  string? Bus { get; }

  /// <summary>
  /// The name of the topic.
  /// </summary>
  string? Topic { get; }
}
