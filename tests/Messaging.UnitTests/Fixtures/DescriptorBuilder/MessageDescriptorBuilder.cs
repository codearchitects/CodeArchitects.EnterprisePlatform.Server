using CodeArchitects.Platform.Messaging.Descriptors;
using Moq;

namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal class MessageDescriptorBuilder : IMessageDescriptorBuilder
{
  private readonly Mock<IMessageDescriptor> _descriptorMock;

  public MessageDescriptorBuilder(MockBehavior behavior)
  {
    _descriptorMock = new(behavior);
  }

  public IMessageDescriptor Descriptor => _descriptorMock.Object;

  public IMessageDescriptorBuilder SetName(string name)
  {
    _descriptorMock
      .Setup(x => x.Name)
      .Returns(name);
    return this;
  }

  public IMessageDescriptorBuilder SetType(Type type)
  {
    _descriptorMock
      .Setup(x => x.Type)
      .Returns(type);
    return this;
  }
}
