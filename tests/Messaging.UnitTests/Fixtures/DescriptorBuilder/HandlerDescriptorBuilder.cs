using CodeArchitects.Platform.Messaging.Descriptors;
using Moq;

namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal class HandlerDescriptorBuilder : IHandlerDescriptorBuilder
{
  private readonly MockBehavior _behavior;
  private readonly Mock<IHandlerDescriptor> _descriptorMock;
  private readonly List<IHandlerIdentityDescriptor> _identityDescriptors;

  public HandlerDescriptorBuilder(MockBehavior behavior)
  {
    _behavior = behavior;
    _descriptorMock = new(behavior);

    _identityDescriptors = new List<IHandlerIdentityDescriptor>();
    _descriptorMock
      .Setup(x => x.IdentityDescriptors)
      .Returns(_identityDescriptors);
  }

  public IHandlerDescriptor Descriptor => _descriptorMock.Object;

  public IHandlerDescriptorBuilder SetConcreteType(Type concreteType)
  {
    _descriptorMock
      .Setup(x => x.ConcreteType)
      .Returns(concreteType);
    return this;
  }

  public IHandlerDescriptorBuilder AddIdentityDescriptor(Func<IHandlerIdentityDescriptorBuilder, IHandlerIdentityDescriptorBuilder> build)
  {
    HandlerIdentityDescriptorBuilder builder = new(_behavior);
    build(builder);
    _identityDescriptors.Add(builder.Descriptor);
    return this;
  }
}
