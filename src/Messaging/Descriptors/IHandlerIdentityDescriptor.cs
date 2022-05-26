namespace CodeArchitects.Platform.Messaging.Descriptors;

internal interface IHandlerIdentityDescriptor
{
  Type InterfaceType { get; }
  Type MessageType { get; }
  Type? ResultType { get; }
  string? Bus { get; }
  string? Topic { get; }
  IReadOnlyCollection<IOutputBindingDescriptor> OutputBindingDescriptors { get; }
}
