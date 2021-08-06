using CodeArchitects.Platform.Infrastructure.Dapr.State;
using Dapr.Client;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Tests.State
{
  public class DaprStateStoreTests
  {
    private const string StoreName = nameof(StoreName);

    private readonly Mock<DaprClient> _daprClientMock;
    private readonly DaprStateStore _sut;

    public DaprStateStoreTests()
    {
      _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
      _sut = new DaprStateStore(_daprClientMock.Object, StoreName);
    }

    [Fact]
    public async Task GetAsync_ShouldCallGetStateAsyncExacltyOnceWithCorrectParameters()
    {
      // Arrange
      const string key = nameof(key);
      _daprClientMock
        .Setup(x => x.GetStateAsync<StateStub>(
          StoreName,                      // storeName
          key,                            // key
          null,                           // consistencyMode
          null,                           // metadata
          It.IsAny<CancellationToken>())) // cancellationToken
        .ReturnsAsync(new StateStub());

      // Act
      await _sut.GetAsync<StateStub>(key, default);

      // Assert
      _daprClientMock.Verify(x => x.GetStateAsync<StateStub>(StoreName, key, null, null, default), Times.Once());
    }

    [Fact]
    public async Task SaveAsync_ShouldCallSaveStateAsyncExacltyOnceWithCorrectParameters()
    {
      // Arrange
      const string key = nameof(key);
      StateStub state = new StateStub();
      _daprClientMock
        .Setup(x => x.SaveStateAsync(
          StoreName,                      // storeName
          key,                            // key
          state,                          // value
          null,                           // consistencyMode
          null,                           // metadata
          It.IsAny<CancellationToken>())) // cancellationToken
        .Returns(Task.CompletedTask);

      // Act
      await _sut.SaveAsync(key, state, default);

      // Assert
      _daprClientMock.Verify(x => x.SaveStateAsync(StoreName, key, state, null, null, default), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallDeleteStateAsyncExacltyOnceWithCorrectParameters()
    {
      // Arrange
      const string key = nameof(key);
      _daprClientMock
        .Setup(x => x.DeleteStateAsync(
          StoreName,                      // storeName
          key,                            // key
          null,                           // consistencyMode
          null,                           // metadata
          It.IsAny<CancellationToken>())) // cancellationToken
        .Returns(Task.CompletedTask);

      // Act
      await _sut.DeleteAsync(key, default);

      // Assert
      _daprClientMock.Verify(x => x.DeleteStateAsync(StoreName, key, null, null, default), Times.Once());
    }

    private class StateStub
    {
      public int Property { get; set; }
    }
  }
}
