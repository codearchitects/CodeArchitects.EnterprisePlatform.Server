using CodeArchitects.Platform.Actors.Dapr.Fixtures.TestModel;
using CodeArchitects.Platform.Actors.TestModel;

namespace CodeArchitects.Platform.Actors.Dapr.Proxy;

public partial class ActorProxyTests
{
  [Theory]
  [StandardActorProxyData]
  internal async Task TaskMethod_ShouldInvokeCorrespondingHostMethod(Mock<IStandardActorHost> actorHostMock, IStandardActor proxy)
  {
    // Arrange
    int arg = 12;

    // Act
    await proxy.TaskMethod(arg);

    // Assert
    actorHostMock.Verify(x => x.TaskMethod_1(arg));
  }

  [Theory]
  [StandardActorProxyData]
  internal async Task TaskTMethod_ShouldInvokeCorrespondingHostMethod(Mock<IStandardActorHost> actorHostMock, IStandardActor proxy)
  {
    // Arrange
    int arg = 12;
    int expected = 42;

    actorHostMock
      .Setup(x => x.TaskMethod_2(arg, CancellationToken.None))
      .ReturnsAsync(expected);

    // Act
    int actual = await proxy.TaskMethod(arg, CancellationToken.None);

    // Assert
    actorHostMock.Verify(x => x.TaskMethod_2(arg, CancellationToken.None));
    actual.Should().Be(expected);
  }

  [Theory]
  [StandardActorProxyData]
  internal async Task ValueTaskMethod_ShouldInvokeCorrespondingHostMethod(Mock<IStandardActorHost> actorHostMock, IStandardActor proxy)
  {
    // Arrange

    // Act
    await proxy.ValueTaskMethod(CancellationToken.None);

    // Assert
    actorHostMock.Verify(x => x.ValueTaskMethod(CancellationToken.None));
  }

  [Theory]
  [StandardActorProxyData]
  internal async Task ValueTaskTMethod_ShouldInvokeCorrespondingHostMethod(Mock<IStandardActorHost> actorHostMock, IStandardActor proxy)
  {
    // Arrange
    string expected = "42";

    actorHostMock
      .Setup(x => x.ValueTaskTMethod())
      .ReturnsAsync(expected);

    // Act
    string actual = await proxy.ValueTaskTMethod();

    // Assert
    actorHostMock.Verify(x => x.ValueTaskTMethod());
    actual.Should().Be(expected);
  }
}
