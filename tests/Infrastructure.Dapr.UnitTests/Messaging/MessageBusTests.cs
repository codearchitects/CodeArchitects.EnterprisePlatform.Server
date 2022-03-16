using CodeArchitects.Platform.Infrastructure.Dapr.Messaging.Fakes;
using Dapr.Client;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

public class MessageBusTests
{
  private const string BusName = nameof(BusName);

  private readonly Mock<DaprClient> _daprClientMock;
  private readonly MessageBus _sut;

  public MessageBusTests()
  {
    _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
    _sut = new MessageBus(_daprClientMock.Object, BusName);
  }

  [Fact]
  public async Task SendAsync_ShouldCallPublishEventAsyncExacltyOnceWithCorrectParameters()
  {
    // Arrange
    _daprClientMock
      .Setup(x => x.PublishEventAsync(
        It.IsAny<string>(),                    // pubsubName
        It.IsAny<string>(),                    // topicName
        It.IsAny<MessageEnvelope<Message1>>(), // data
        It.IsAny<CancellationToken>()))        // cancellationToken
      .Returns(Task.CompletedTask);
    Message1 message = new Message1(default);

    // Act
    await _sut.SendAsync(message);

    // Assert
    _daprClientMock.Verify(x => x.PublishEventAsync(BusName, "__global", It.IsAny<MessageEnvelope<Message1>>(), CancellationToken.None), Times.Once());
  }

  [Fact]
  public async Task SendAsync_WithTopic_ShouldCallPublishEventAsyncExacltyOnceWithCorrectParameters()
  {
    // Arrange
    string topic = "topic";
    _daprClientMock
      .Setup(x => x.PublishEventAsync(
        It.IsAny<string>(),                    // pubsubName
        It.IsAny<string>(),                    // topicName
        It.IsAny<MessageEnvelope<Message1>>(), // data
        It.IsAny<CancellationToken>()))        // cancellationToken
      .Returns(Task.CompletedTask);
    Message1 message = new Message1(default);

    // Act
    await _sut.SendAsync(topic, message);

    // Assert
    _daprClientMock.Verify(x => x.PublishEventAsync(BusName, topic, It.IsAny<MessageEnvelope<Message1>>(), CancellationToken.None), Times.Once());
  }
}
