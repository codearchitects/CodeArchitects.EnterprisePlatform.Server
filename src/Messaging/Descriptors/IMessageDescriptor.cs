namespace CodeArchitects.Platform.Messaging.Descriptors;

internal interface IMessageDescriptor
{
  Type Type { get; }
  string Name { get; }
}
