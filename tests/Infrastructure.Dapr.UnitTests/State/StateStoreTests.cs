using Dapr.Client;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.State;

public class StateStoreTests
{
  private const string s_storeName = nameof(s_storeName);

  private readonly Mock<DaprClient> _daprClientMock;
  private readonly StateStore _sut;

  public StateStoreTests()
  {
    _daprClientMock = new Mock<DaprClient>(behavior: MockBehavior.Strict);
    _sut = new StateStore(_daprClientMock.Object, s_storeName);
  }

  [Fact]
  public async Task GetAsync_ShouldCallGetStateAsyncExacltyOnceWithCorrectParameters()
  {
    // Arrange
    const string key = nameof(key);
    _daprClientMock
      .Setup(x => x.GetStateAsync<StateStub>(
        s_storeName,                    // storeName
        key,                            // key
        null,                           // consistencyMode
        null,                           // metadata
        It.IsAny<CancellationToken>())) // cancellationToken
      .ReturnsAsync(new StateStub());

    // Act
    await _sut.GetAsync<StateStub>(key, default);

    // Assert
    _daprClientMock.Verify(x => x.GetStateAsync<StateStub>(s_storeName, key, null, null, default), Times.Once());
  }

  [Fact]
  public async Task SaveAsync_ShouldCallSaveStateAsyncExacltyOnceWithCorrectParameters()
  {
    // Arrange
    const string key = nameof(key);
    StateStub state = new StateStub();
    _daprClientMock
      .Setup(x => x.SaveStateAsync(
        s_storeName,                    // storeName
        key,                            // key
        state,                          // value
        null,                           // consistencyMode
        null,                           // metadata
        It.IsAny<CancellationToken>())) // cancellationToken
      .Returns(Task.CompletedTask);

    // Act
    await _sut.SaveAsync(key, state, default);

    // Assert
    _daprClientMock.Verify(x => x.SaveStateAsync(s_storeName, key, state, null, null, default), Times.Once());
  }

  [Fact]
  public async Task DeleteAsync_ShouldCallDeleteStateAsyncExacltyOnceWithCorrectParameters()
  {
    // Arrange
    const string key = nameof(key);
    _daprClientMock
      .Setup(x => x.DeleteStateAsync(
        s_storeName,                    // storeName
        key,                            // key
        null,                           // consistencyMode
        null,                           // metadata
        It.IsAny<CancellationToken>())) // cancellationToken
      .Returns(Task.CompletedTask);

    // Act
    await _sut.DeleteAsync(key, default);

    // Assert
    _daprClientMock.Verify(x => x.DeleteStateAsync(s_storeName, key, null, null, default), Times.Once());
  }

  private class StateStub
  {
    public int Property { get; set; }
  }
}
