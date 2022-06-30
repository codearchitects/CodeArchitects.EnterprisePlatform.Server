using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeArchitects.Platform.Infrastructure.Dapr.State;

public class StateStoreResolverTests
{
  private readonly Mock<DaprClient> _daprClientMock;
  private readonly StateStoreResolver _sut;

  public StateStoreResolverTests()
  {
    _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
    _sut = new StateStoreResolver(_daprClientMock.Object, new DaprStateOptions(), Mock.Of<ILogger<StateStoreResolver>>());
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
