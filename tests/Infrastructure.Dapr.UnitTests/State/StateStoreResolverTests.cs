using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Infrastructure.Dapr.State;

public class StateStoreResolverTests
{
  private readonly Mock<ILogger> _loggerMock;
  private readonly Mock<IStoreInfo> _infoMock;

  private readonly StateStoreResolver _sut;

  public StateStoreResolverTests()
  {
    _loggerMock = new(MockBehavior.Loose);
    _infoMock = new(MockBehavior.Strict);

    _infoMock
      .Setup(x => x.IsStoreKnown(It.IsAny<string>()))
      .Returns(true);

    _sut = new StateStoreResolver(Mock.Of<DaprClient>(), _infoMock.Object, _loggerMock.Object);
  }

  [Fact]
  public void Resolve_ShouldCreateMessageStoreOnlyOnceForSameStoreName()
  {
    // Arrange
    string storeName = "statestore";

    // Act
    IStateStore store1 = _sut.Resolve(storeName);
    IStateStore store2 = _sut.Resolve(storeName);

    // Assert
    store1.Should().BeSameAs(store2);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  public void Resolve_ShouldThrowArgumentException_WhenStoreNameIsNullOrWhitespace(string storeName)
  {
    // Arrange

    // Act
    Func<IStateStore> act = () => _sut.Resolve(storeName);

    // Assert
    act.Should().ThrowExactly<ArgumentException>();
  }

  [Fact]
  public void Resolve_ShouldLogWarning_WhenStoreIsNotKnown()
  {
    // Arrange
    string storeName = "statestore";
    _infoMock
      .Setup(x => x.IsStoreKnown(storeName))
      .Returns(false);

    // Act
    _ = _sut.Resolve(storeName);

    // Assert
    _loggerMock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()));
  }

  [Fact]
  public void Resolve_ShouldNotLogWarning_WhenStoreIsKnown()
  {
    // Arrange
    string storeName = "statestore";

    // Act
    _ = _sut.Resolve(storeName);

    // Assert
    _loggerMock.VerifyNoOtherCalls();
  }
}
