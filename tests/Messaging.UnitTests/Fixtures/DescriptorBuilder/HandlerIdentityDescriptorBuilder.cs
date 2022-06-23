using CodeArchitects.Platform.Messaging.Descriptors;
using Moq;

namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal class HandlerIdentityDescriptorBuilder : IHandlerIdentityDescriptorBuilder
{
  private readonly MockBehavior _behavior;
  private readonly Mock<IHandlerIdentityDescriptor> _descriptorMock;
  private readonly List<IOutputBindingDescriptor> _outputBindingDescriptors;

  public HandlerIdentityDescriptorBuilder(MockBehavior behavior)
  {
    _behavior = behavior;
    _descriptorMock = new(behavior);

    _outputBindingDescriptors = new List<IOutputBindingDescriptor>();
    _descriptorMock
      .Setup(x => x.OutputBindingDescriptors)
      .Returns(_outputBindingDescriptors);
  }

  public IHandlerIdentityDescriptor Descriptor => _descriptorMock.Object;

  public IHandlerIdentityDescriptorBuilder SetInterfaceType(Type interfaceType)
  {
    _descriptorMock
      .Setup(x => x.InterfaceType)
      .Returns(interfaceType);
    return this;
  }

  public IHandlerIdentityDescriptorBuilder SetConcreteType(Type concreteType)
  {
    _descriptorMock
      .Setup(x => x.ConcreteType)
      .Returns(concreteType);
    return this;
  }

  public IHandlerIdentityDescriptorBuilder SetMessageType(Type messageType)
  {
    _descriptorMock
      .Setup(x => x.MessageType)
      .Returns(messageType);
    return this;
  }

  public IHandlerIdentityDescriptorBuilder SetResultType(Type resultType)
  {
    _descriptorMock
      .Setup(x => x.ResultType)
      .Returns(resultType);
    return this;
  }

  public IHandlerIdentityDescriptorBuilder SetBus(string? bus)
  {
    _descriptorMock
      .Setup(x => x.Bus)
      .Returns(bus);
    return this;
  }

  public IHandlerIdentityDescriptorBuilder SetTopic(string? topic)
  {
    _descriptorMock
      .Setup(x => x.Topic)
      .Returns(topic);
    return this;
  }

  public IHandlerIdentityDescriptorBuilder AddOutputBindingDescriptor(Func<IOutputBindingDescriptorBuilder, IOutputBindingDescriptorBuilder> build)
  {
    OutputBindingDescriptorBuilder builder = new(_behavior);
    build(builder);
    _outputBindingDescriptors.Add(builder.Descriptor);
    return this;
  }
}
