namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal interface IOutputBindingDescriptorBuilder
{
  IOutputBindingDescriptorBuilder SetMetadataType(Type metadataType);

  IOutputBindingDescriptorBuilder SetMetadataObject(object metadataObject);
}