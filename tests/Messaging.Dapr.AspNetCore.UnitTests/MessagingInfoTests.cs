using CodeArchitects.Platform.Messaging.AspNetCore.Utils;
using static CodeArchitects.Platform.Messaging.Dapr.AspNetCore.MessagingInfoFixture;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

public class MessagingInfoTests
{
  private readonly Mock<IMessageBiMap> _messageMapMock;
  private readonly HashSet<string> _busNames;

  public MessagingInfoTests()
  {
    _messageMapMock = new(MockBehavior.Strict);
    _busNames = new();
  }

  [Fact]
  public void GetMessageName_ShouldReturnNameFromMessageMap_WhenMessageMapCanFindMessage()
  {
    // Arrange
    Type messageType = typeof(Message);
    string? expected = "messageName";
    _messageMapMock
      .Setup(x => x.TryGetValue(messageType, out expected))
      .Returns(true);

    MessagingInfo sut = new(_messageMapMock.Object, _busNames, null);

    // Act
    string actual = sut.GetMessageName(messageType);

    // Assert
    actual.Should().Be(expected);
  }

  [Fact]
  public void GetMessageName_ShouldReturnTypeName_WhenMessageMapCannotFindMessage()
  {
    // Arrange
    Type messageType = typeof(Message);
    string? messageName = null;
    _messageMapMock
      .Setup(x => x.TryGetValue(messageType, out messageName))
      .Returns(false);

    MessagingInfo sut = new(_messageMapMock.Object, _busNames, null);

    // Act
    string actual = sut.GetMessageName(messageType);

    // Assert
    actual.Should().Be(messageType.Name);
  }

  [Fact]
  public void IsBusKnown_ShouldReturnTrue_WhenBusIsDefaultBus()
  {
    // Arrange
    string busName = "busName";

    MessagingInfo sut = new(_messageMapMock.Object, _busNames, busName);

    // Act
    bool isBusKnown = sut.IsBusKnown(busName);

    // Assert
    isBusKnown.Should().BeTrue();
  }

  [Fact]
  public void IsBusKnown_ShouldReturnTrue_WhenBusIsInBusNames()
  {
    // Arrange
    string busName = "busName";
    _busNames.Add(busName);

    MessagingInfo sut = new(_messageMapMock.Object, _busNames, null);

    // Act
    bool isBusKnown = sut.IsBusKnown(busName);

    // Assert
    isBusKnown.Should().BeTrue();
  }

  [Fact]
  public void IsBusKnown_ShouldReturnFalse_WhenBusIsNotDefaultBusAndIsNotInBusNames()
  {
    // Arrange
    string busName = "busName";

    MessagingInfo sut = new(_messageMapMock.Object, _busNames, null);

    // Act
    bool isBusKnown = sut.IsBusKnown(busName);

    // Assert
    isBusKnown.Should().BeFalse();
  }

  [Fact]
  public void GetDefaultBus_ShouldReturnDefaultBus_WhenDefaultBusIsNotNull()
  {
    // Arrange
    string expected = "defaultBus";

    MessagingInfo sut = new(_messageMapMock.Object, _busNames, expected);

    // Act
    string? actual = sut.GetDefaultBus();

    // Assert
    actual.Should().Be(expected);
  }

  [Fact]
  public void GetDefaultBus_ShouldReturnSingleElement_WhenDefaultBusIsNullAndKnownBusNamesContainsOneElement()
  {
    // Arrange
    string expected = "defaultBus";
    _busNames.Add(expected);

    MessagingInfo sut = new(_messageMapMock.Object, _busNames, null);

    // Act
    string? actual = sut.GetDefaultBus();

    // Assert
    actual.Should().Be(expected);
  }

  [Fact]
  public void GetDefaultBus_ShouldReturnNull_WhenDefaultBusIsNullAndKnownBusNamesContainsNoElements()
  {
    // Arrange
    MessagingInfo sut = new(_messageMapMock.Object, _busNames, null);

    // Act
    string? actual = sut.GetDefaultBus();

    // Assert
    actual.Should().Be(null);
  }
}
