namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal interface IOutputBindingDescriptorBuilder
{
  IOutputBindingDescriptorBuilder SetMetadataType(Type metadataType);

  IOutputBindingDescriptorBuilder SetIsTypeFiltered(bool isTypeFiltered);

  IOutputBindingDescriptorBuilder SetMetadataObject(object metadataObject);
}