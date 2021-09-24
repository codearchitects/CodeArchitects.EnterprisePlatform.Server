using CodeArchitects.Platform.Infrastructure.Dapr.Tests;
using Dapr.Client;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging
{
  public class DaprMessageBusTests
  {
    private const string BusName = nameof(BusName);

    private readonly Mock<DaprClient> _daprClientMock;
    private readonly DaprMessageBus _sut;

    public DaprMessageBusTests()
    {
      _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
      _sut = new DaprMessageBus(_daprClientMock.Object, BusName);
    }

    [Fact]
    public async Task SendAsync_ShouldCallPublishEventAsyncExacltyOnceWithCorrectParameters()
    {
      // Arrange
      _daprClientMock
        .Setup(x => x.PublishEventAsync(
          It.IsAny<string>(),             // pubsubName
          It.IsAny<string>(),             // topicName
          It.IsAny<FakeMessage1>(),       // data
          It.IsAny<CancellationToken>())) // cancellationToken
        .Returns(Task.CompletedTask);
      FakeMessage1 message = new FakeMessage1(default);

      // Act
      await _sut.SendAsync(message);

      // Assert
      _daprClientMock.Verify(x => x.PublishEventAsync(BusName, DaprTopic.Make(null, typeof(FakeMessage1)), message, default), Times.Once());
    }

    [Fact]
    public async Task SendAsync_WithMetadataArgShouldCallPublishEventAsyncExacltyOnceWithCorrectParameters()
    {
      // Arrange
      const string topic = "topic";
      _daprClientMock
        .Setup(x => x.PublishEventAsync(
          It.IsAny<string>(),                     // pubsubName
          It.IsAny<string>(),                     // topicName
          It.IsAny<FakeMessage1>(),               // data
          It.IsAny<Dictionary<string, string>>(), // metadata
          It.IsAny<CancellationToken>()))         // cancellationToken
        .Returns(Task.CompletedTask);
      FakeMessage1 message = new FakeMessage1(default);
      DaprMetadata metadata = new DaprMetadata
      {
        TimeToLiveInSeconds = 3,
        RawPayload = true,
        Topic = topic
      };

      // Act
      await _sut.SendAsync(message, metadata);

      // Assert
      _daprClientMock.Verify(x => x.PublishEventAsync(BusName, DaprTopic.Make(topic, typeof(FakeMessage1)), message, metadata.MetadataDictionary, default), Times.Once());
    }
  }
}
