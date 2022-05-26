using CodeArchitects.Platform.Messaging.Descriptors;

namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal interface IHandlerDescriptorBuilder
{
  IHandlerDescriptor Descriptor { get; }

  IHandlerDescriptorBuilder SetConcreteType(Type concreteType);

  IHandlerDescriptorBuilder AddIdentityDescriptor(Func<IHandlerIdentityDescriptorBuilder, IHandlerIdentityDescriptorBuilder> build);
}
