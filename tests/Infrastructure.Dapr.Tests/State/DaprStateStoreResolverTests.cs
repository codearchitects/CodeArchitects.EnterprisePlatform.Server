using CodeArchitects.Platform.Infrastructure.Dapr.State;
using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using FluentAssertions;
using Moq;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Tests.State
{
  public class DaprStateStoreResolverTests
  {
    private readonly Mock<DaprClient> _daprClientMock;
    private readonly DaprStateStoreResolver _sut;

    public DaprStateStoreResolverTests()
    {
      _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
      _sut = new DaprStateStoreResolver(_daprClientMock.Object);
    }

    [Fact]
    public void Resolve_ShouldCreateStoreOnlyOnce()
    {
      // Arrange
      const string storeName = nameof(storeName);

      // Act
      IStateStore store1 = _sut.Resolve(storeName);
      IStateStore store2 = _sut.Resolve(storeName);

      // Assert
      store1.Should().BeSameAs(store2);
    }
  }
}
