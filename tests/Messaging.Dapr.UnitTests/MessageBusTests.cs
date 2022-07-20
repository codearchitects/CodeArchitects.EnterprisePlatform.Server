using CodeArchitects.Platform.Messaging.Dapr.Fixtures;
using Dapr;
using Dapr.Client;
using FluentAssertions;
using Moq;

namespace CodeArchitects.Platform.Messaging.Dapr;

public class MessageBusTests
{
  private readonly Mock<DaprClient> _daprMock;
  private readonly Mock<IMessagingInfo> _infoMock;
  private readonly string _name;
  private readonly MessageBus _sut;

  public MessageBusTests()
  {
    _daprMock = new(MockBehavior.Loose);
    _infoMock = new(MockBehavior.Strict);
    _name = "messagebus";
    _sut = new MessageBus(_daprMock.Object, _infoMock.Object, _name);
  }

  [Fact]
  public async Task SendAsyncNoTopic_ShouldCallPublishEventAsyncWithCorrectArguments_WhenMessageIsNotNull()
  {
    // Arrange
    string messageName = "msg1";
    Message1 message = new Message1();

    _infoMock
      .Setup(x => x.GetMessageName(typeof(Message1)))
      .Returns(messageName);

    // Act
    await _sut.SendAsync(message, CancellationToken.None);

    // Assert
    _daprMock.Verify(x => x.PublishEventAsync(_name, MessageBus.DefaultTopic, It.Is<CloudEvent<Message1>>(evt => evt.Data == message && evt.Type == messageName), It.IsAny<CancellationToken>()));
  }

  [Fact]
  public async Task SendAsyncNoTopic_ShouldThrowArgumentNullException_WhenMessageIsNull()
  {
    // Arrange

    // Act
    Func<Task> act = async () => await _sut.SendAsync<Message1>(null!, CancellationToken.None);

    // Assert
    await act.Should().ThrowExactlyAsync<ArgumentNullException>();
  }

  [Fact]
  public async Task SendAsyncWithTopic_ShouldCallPublishEventAsyncWithCorrectArguments_WhenMessageIsNotNull()
  {
    // Arrange
    string topic = "topic";
    string messageName = "msg1";
    Message1 message = new Message1();

    _infoMock
      .Setup(x => x.GetMessageName(typeof(Message1)))
      .Returns(messageName);

    // Act
    await _sut.SendAsync(topic, message, CancellationToken.None);

    // Assert
    _daprMock.Verify(x => x.PublishEventAsync(_name, topic, It.Is<CloudEvent<Message1>>(evt => evt.Data == message && evt.Type == messageName), It.IsAny<CancellationToken>()));
  }

  [Fact]
  public async Task SendAsyncWithTopic_ShouldThrowArgumentNullException_WhenMessageIsNull()
  {
    // Arrange

    // Act
    Func<Task> act = async () => await _sut.SendAsync<Message1>("topic", null!, CancellationToken.None);

    // Assert
    await act.Should().ThrowExactlyAsync<ArgumentNullException>();
  }

  [Fact]
  public async Task SendAsyncWithTopic_ShouldThrowArgumentNullException_WhenTopicIsNull()
  {
    // Arrange
    Message1 message = new Message1();

    // Act
    Func<Task> act = async () => await _sut.SendAsync(null!, message, CancellationToken.None);

    // Assert
    await act.Should().ThrowExactlyAsync<ArgumentNullException>();
  }
}
