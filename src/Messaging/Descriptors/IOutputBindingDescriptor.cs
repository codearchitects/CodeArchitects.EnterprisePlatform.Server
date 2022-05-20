namespace CodeArchitects.Platform.Messaging.Descriptors;

internal interface IOutputBindingDescriptor
{
  Type MetadataType { get; }
  object Metadata { get; }
}
