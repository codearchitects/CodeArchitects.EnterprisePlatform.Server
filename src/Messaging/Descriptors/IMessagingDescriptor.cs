namespace CodeArchitects.Platform.Messaging.Descriptors;

internal interface IMessagingDescriptor
{
  IEnumerable<IHandlerDescriptor> HandlerDescriptors { get; }
  IReadOnlyCollection<HandlerDiagnostics> Diagnostics { get; }
}
