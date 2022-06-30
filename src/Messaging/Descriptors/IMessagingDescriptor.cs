namespace CodeArchitects.Platform.Messaging.Descriptors;

/// <summary>
/// Describes a group of message handlers.
/// </summary>
internal interface IMessagingDescriptor
{
  /// <summary>
  /// The descriptors of the handlers.
  /// </summary>
  IEnumerable<IHandlerDescriptor> HandlerDescriptors { get; }

  /// <summary>
  /// The descriptors of the messages the handlers handle.
  /// </summary>
  IEnumerable<IMessageDescriptor> MessageDescriptors { get; }

  /// <summary>
  /// A collection of diagnostics produced by the handler description.
  /// </summary>
  IReadOnlyCollection<HandlerDiagnostics> Diagnostics { get; }
}
