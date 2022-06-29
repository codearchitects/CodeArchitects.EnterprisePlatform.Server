namespace CodeArchitects.Platform.Messaging.Descriptors;

internal interface IMessagingDescriptor
{
  IEnumerable<IHandlerDescriptor> HandlerDescriptors { get; }
  IEnumerable<IMessageDescriptor> MessageDescriptors { get; }
  IReadOnlyCollection<HandlerDiagnostics> Diagnostics { get; }
}
