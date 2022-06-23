namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal interface IHandlerIdentityDescriptorBuilder
{
  IHandlerIdentityDescriptorBuilder SetInterfaceType(Type interfaceType);

  IHandlerIdentityDescriptorBuilder SetConcreteType(Type concreteType);

  IHandlerIdentityDescriptorBuilder SetMessageType(Type messageType);
  
  IHandlerIdentityDescriptorBuilder SetResultType(Type resultType);

  IHandlerIdentityDescriptorBuilder SetBus(string? bus);

  IHandlerIdentityDescriptorBuilder SetTopic(string? topic);

  IHandlerIdentityDescriptorBuilder AddOutputBindingDescriptor(Func<IOutputBindingDescriptorBuilder, IOutputBindingDescriptorBuilder> build);
}