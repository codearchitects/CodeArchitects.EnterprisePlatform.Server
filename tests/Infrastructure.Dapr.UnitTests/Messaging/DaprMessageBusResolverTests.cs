using CodeArchitects.Platform.Infrastructure.Messaging;
using Dapr.Client;
using FluentAssertions;
using Moq;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging
{
  public class DaprMessageBusResolverTests
  {
    private readonly Mock<DaprClient> _daprClientMock;
    private readonly DaprMessageBusResolver _sut;

    public DaprMessageBusResolverTests()
    {
      _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
      _sut = new DaprMessageBusResolver(_daprClientMock.Object);
    }

    [Fact]
    public void Resolve_ShouldCreateBusOnlyOnce()
    {
      // Arrange
      const string busName = nameof(busName);

      // Act
      IMessageBus bus1 = _sut.Resolve(busName);
      IMessageBus bus2 = _sut.Resolve(busName);

      // Assert
      bus1.Should().BeSameAs(bus2);
    }

    [Fact]
    public void Resolve_WithMetadataShouldCreateBusOnlyOnce()
    {
      // Arrange
      const string busName = nameof(busName);

      // Act
      IMessageBus<DaprMetadata> bus1 = _sut.Resolve(busName);
      IMessageBus<DaprMetadata> bus2 = _sut.Resolve(busName);

      // Assert
      bus1.Should().BeSameAs(bus2);
    }
  }
}
