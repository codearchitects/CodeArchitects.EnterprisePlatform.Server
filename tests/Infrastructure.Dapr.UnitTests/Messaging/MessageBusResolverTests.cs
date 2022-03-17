using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using FluentAssertions;
using Moq;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging;

public class MessageBusResolverTests
{
  private readonly Mock<DaprClient> _daprClientMock;
  private readonly MessageBusResolver _sut;

  public MessageBusResolverTests()
  {
    _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
    _sut = new MessageBusResolver(_daprClientMock.Object, new DaprConfiguration());
  }

  [Fact]
  public void Resolve_ShouldCreateBusOnlyOnce()
  {
    // Arrange
    const string busName = "busName";

    // Act
    IMessageBus bus1 = _sut.Resolve(busName);
    IMessageBus bus2 = _sut.Resolve(busName);

    // Assert
    bus1.Should().BeSameAs(bus2);
  }
}
