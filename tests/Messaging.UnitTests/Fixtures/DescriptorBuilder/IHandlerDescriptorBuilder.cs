namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal interface IHandlerDescriptorBuilder
{
  IHandlerDescriptorBuilder SetBus(string bus);

  IHandlerDescriptorBuilder SetTopic(string topic);

  IHandlerDescriptorBuilder SetInterfaceType(Type interfaceType);

  IHandlerDescriptorBuilder SetConcreteType(Type concreteType);

  IHandlerDescriptorBuilder SetMessageType(Type messageType);

  IHandlerDescriptorBuilder SetResultType(Type resultType);

  IHandlerDescriptorBuilder SetHasResult(bool hasResult);

  IHandlerDescriptorBuilder SetHasUnionResult(bool hasUnionResult);

  IHandlerDescriptorBuilder SetResultTypes(IReadOnlyList<Type> resultTypes);

  IHandlerDescriptorBuilder AddOutputBindingDescriptor(Func<IOutputBindingDescriptorBuilder, IOutputBindingDescriptorBuilder> build);
}