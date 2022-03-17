using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using FluentAssertions;
using Moq;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.State;

public class StateStoreResolverTests
{
  private readonly Mock<DaprClient> _daprClientMock;
  private readonly StateStoreResolver _sut;

  public StateStoreResolverTests()
  {
    _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
    _sut = new StateStoreResolver(_daprClientMock.Object, new DaprConfiguration());
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
