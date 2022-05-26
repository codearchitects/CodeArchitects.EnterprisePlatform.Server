namespace CodeArchitects.Platform.Messaging.Descriptors;

internal interface IHandlerDescriptor
{
  Type ConcreteType { get; }
  IReadOnlyCollection<IHandlerIdentityDescriptor> IdentityDescriptors { get; }
}
