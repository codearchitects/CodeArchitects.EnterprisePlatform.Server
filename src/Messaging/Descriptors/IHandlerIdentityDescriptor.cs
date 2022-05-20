namespace CodeArchitects.Platform.Messaging.Descriptors;

internal interface IHandlerIdentityDescriptor
{
  Type InterfaceType { get; }
  Type MessageType { get; }
  Type? ResultType { get; }
  IReadOnlyCollection<IOutputBindingDescriptor> OutputBindings { get; }
  string Bus { get; }
  string Topic { get; }
}
