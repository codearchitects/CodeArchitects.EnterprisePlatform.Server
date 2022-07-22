using CodeArchitects.Platform.Messaging.Descriptors;
using Moq;

namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal class HandlerDescriptorBuilder : IHandlerDescriptorBuilder
{
  private readonly MockBehavior _behavior;
  private readonly Mock<IHandlerDescriptor> _descriptorMock;
  private readonly List<IOutputBindingDescriptor> _outputBindingDescriptors;

  public HandlerDescriptorBuilder(MockBehavior behavior)
  {
    _behavior = behavior;
    _descriptorMock = new(behavior);

    _outputBindingDescriptors = new List<IOutputBindingDescriptor>();
    _descriptorMock
      .Setup(x => x.OutputBindingDescriptors)
      .Returns(_outputBindingDescriptors);
  }

  public IHandlerDescriptor Descriptor => _descriptorMock.Object;

  public IHandlerDescriptorBuilder SetBus(string bus)
  {
    _descriptorMock
      .Setup(x => x.Bus)
      .Returns(bus);
    return this;
  }

  public IHandlerDescriptorBuilder SetTopic(string topic)
  {
    _descriptorMock
      .Setup(x => x.Topic)
      .Returns(topic);
    return this;
  }

  public IHandlerDescriptorBuilder SetInterfaceType(Type interfaceType)
  {
    _descriptorMock
      .Setup(x => x.InterfaceType)
      .Returns(interfaceType);
    return this;
  }

  public IHandlerDescriptorBuilder SetConcreteType(Type concreteType)
  {
    _descriptorMock
      .Setup(x => x.ConcreteType)
      .Returns(concreteType);
    return this;
  }

  public IHandlerDescriptorBuilder SetMessageType(Type messageType)
  {
    _descriptorMock
      .Setup(x => x.MessageType)
      .Returns(messageType);
    return this;
  }

  public IHandlerDescriptorBuilder SetResultType(Type resultType)
  {
    _descriptorMock
      .Setup(x => x.ResultType)
      .Returns(resultType);
    return this;
  }

  public IHandlerDescriptorBuilder SetHasResult(bool hasResult)
  {
    _descriptorMock
      .Setup(x => x.HasResult)
      .Returns(hasResult);
    return this;
  }

  public IHandlerDescriptorBuilder SetHasUnionResult(bool hasUnionResult)
  {
    _descriptorMock
      .Setup(x => x.HasUnionResult)
      .Returns(hasUnionResult);
    return this;
  }

  public IHandlerDescriptorBuilder SetResultTypes(IReadOnlyList<Type> resultTypes)
  {
    _descriptorMock
      .Setup(x => x.ResultTypes)
      .Returns(resultTypes);
    return this;
  }

  public IHandlerDescriptorBuilder AddOutputBindingDescriptor(Func<IOutputBindingDescriptorBuilder, IOutputBindingDescriptorBuilder> build)
  {
    OutputBindingDescriptorBuilder builder = new(_behavior);
    build(builder);
    _outputBindingDescriptors.Add(builder.Descriptor);
    return this;
  }
}
