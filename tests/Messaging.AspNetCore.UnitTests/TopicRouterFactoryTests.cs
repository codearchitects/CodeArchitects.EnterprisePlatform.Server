using CodeArchitects.Platform.Messaging.AspNetCore.Fixtures;
using CodeArchitects.Platform.Messaging.AspNetCore.Utils;
using CodeArchitects.Platform.Messaging.Descriptors;
using FluentAssertions;
using Moq;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

public class TopicRouterFactoryTests
{
  private readonly Mock<IHandlerDelegateFactory> _delegateFactoryMock;
  private readonly Mock<IMessageBiMap> _messageMapMock;
  private readonly TopicRouterFactory _sut;

  public TopicRouterFactoryTests()
  {
    _delegateFactoryMock = new(MockBehavior.Strict);
    _messageMapMock = new(MockBehavior.Strict);
    _sut = new TopicRouterFactory(_delegateFactoryMock.Object, _messageMapMock.Object);

    _delegateFactoryMock
      .Setup(x => x.CreateHandlerDelegate(It.IsAny<IHandlerDescriptor>()))
      .Returns(Mock.Of<HandlerDelegate>());
  }

  [Fact]
  public void CreateRouter_ShouldCreateTopicRouter_WhenThereAreNoDuplicateDescriptors()
  {
    // Arrange
    IHandlerDescriptor[] descriptors = new[]
    {
      Mock.Of<IHandlerDescriptor>(descriptor => descriptor.MessageType == typeof(Message1)),
      Mock.Of<IHandlerDescriptor>(descriptor => descriptor.MessageType == typeof(Message2))
    };
    _messageMapMock
      .Setup(x => x[typeof(Message1)])
      .Returns(nameof(Message1));
    _messageMapMock
      .Setup(x => x[typeof(Message2)])
      .Returns(nameof(Message2));

    // Act
    TopicRouter router = _sut.CreateRouter(descriptors);

    // Assert
    _delegateFactoryMock.Verify(x => x.CreateHandlerDelegate(descriptors[0]));
    _delegateFactoryMock.Verify(x => x.CreateHandlerDelegate(descriptors[1]));
    _messageMapMock.Verify(x => x[typeof(Message1)]);
    _messageMapMock.Verify(x => x[typeof(Message2)]);
  }

  [Fact]
  public void CreateRouter_ShouldThrowInvalidOperationException_WhenThereAreDuplicateDescriptors()
  {
    // Arrange
    IHandlerDescriptor[] descriptors = new[]
    {
      Mock.Of<IHandlerDescriptor>(descriptor => descriptor.MessageType == typeof(Message1)),
      Mock.Of<IHandlerDescriptor>(descriptor => descriptor.MessageType == typeof(Message1))
    };
    _messageMapMock
      .Setup(x => x[typeof(Message1)])
      .Returns(nameof(Message1));

    // Act
    Func<TopicRouter> act = () => _sut.CreateRouter(descriptors);

    // Assert
    act.Should().ThrowExactly<InvalidOperationException>();
  }
}
