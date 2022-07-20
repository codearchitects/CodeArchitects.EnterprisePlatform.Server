namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal interface IMessagingDescriptorBuilder
{
  IMessagingDescriptorBuilder AddHandlerDescriptor(Func<IHandlerDescriptorBuilder, IHandlerDescriptorBuilder> build);

  IMessagingDescriptorBuilder AddMessageDescriptor(Func<IMessageDescriptorBuilder, IMessageDescriptorBuilder> build);
}
