using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Messaging.Dapr;

public class MessageBusResolverTests
{
  private readonly Mock<ILogger> _loggerMock;
  private readonly Mock<IMessagingInfo> _infoMock;

  private readonly MessageBusResolver _sut;

  public MessageBusResolverTests()
  {
    _loggerMock = new(MockBehavior.Loose);
    _infoMock = new(MockBehavior.Strict);

    _infoMock
      .Setup(x => x.IsBusKnown(It.IsAny<string>()))
      .Returns(true);

    _sut = new MessageBusResolver(Mock.Of<DaprClient>(), _infoMock.Object, _loggerMock.Object);
  }

  [Fact]
  public void Resolve_ShouldCreateMessageBusOnlyOnceForSameBusName()
  {
    // Arrange
    string busName = "messagebus";

    // Act
    IMessageBus bus1 = _sut.Resolve(busName);
    IMessageBus bus2 = _sut.Resolve(busName);

    // Assert
    bus1.Should().BeSameAs(bus2);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  public void Resolve_ShouldThrowArgumentException_WhenBusNameIsNullOrWhitespace(string busName)
  {
    // Arrange

    // Act
    Func<IMessageBus> act = () => _sut.Resolve(busName);

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }

  [Fact]
  public void Resolve_ShouldLogWarning_WhenBusIsNotKnown()
  {
    // Arrange
    string busName = "messagebus";
    _infoMock
      .Setup(x => x.IsBusKnown(busName))
      .Returns(false);

    // Act
    _ = _sut.Resolve(busName);

    // Assert
    _loggerMock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()));
  }

  [Fact]
  public void Resolve_ShouldNotLogWarning_WhenBusIsKnown()
  {
    // Arrange
    string busName = "messagebus";

    // Act
    _ = _sut.Resolve(busName);

    // Assert
    _loggerMock.VerifyNoOtherCalls();
  }
}
