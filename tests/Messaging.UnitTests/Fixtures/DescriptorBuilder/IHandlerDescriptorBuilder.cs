namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal interface IHandlerDescriptorBuilder
{
  IHandlerDescriptorBuilder SetBus(string bus);

  IHandlerDescriptorBuilder SetTopic(string topic);

  IHandlerDescriptorBuilder SetInterfaceType(Type interfaceType);

  IHandlerDescriptorBuilder SetConcreteType(Type concreteType);

  IHandlerDescriptorBuilder SetMessageType(Type messageType);
  
  IHandlerDescriptorBuilder SetResultType(Type resultType);

  IHandlerDescriptorBuilder AddOutputBindingDescriptor(Func<IOutputBindingDescriptorBuilder, IOutputBindingDescriptorBuilder> build);
}