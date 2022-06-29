using CodeArchitects.Platform.Messaging.Descriptors;
using Moq;

namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal class MessagingDescriptorBuilder : IMessagingDescriptorBuilder
{
  private readonly MockBehavior _behavior;
  private readonly Mock<IMessagingDescriptor> _descriptorMock;
  private readonly List<IHandlerDescriptor> _handlerDescriptors;

  public MessagingDescriptorBuilder(MockBehavior behavior)
  {
    _behavior = behavior;
    _descriptorMock = new(behavior);
    _handlerDescriptors = new();

    _descriptorMock
      .Setup(x => x.HandlerDescriptors)
      .Returns(_handlerDescriptors);
  }

  public IMessagingDescriptor Descriptor => _descriptorMock.Object;

  public IMessagingDescriptorBuilder AddHandlerDescriptor(Func<IHandlerDescriptorBuilder, IHandlerDescriptorBuilder> build)
  {
    HandlerDescriptorBuilder builder = new(_behavior);
    build(builder);
    _handlerDescriptors.Add(builder.Descriptor);
    return this;
  }

  public IMessagingDescriptorBuilder SetDiagnostics(IReadOnlyCollection<HandlerDiagnostics> diagnostics)
  {
    _descriptorMock
      .Setup(x => x.Diagnostics)
      .Returns(diagnostics);
    return this;
  }
}
