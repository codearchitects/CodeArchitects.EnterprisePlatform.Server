using CodeArchitects.Platform.Actors.TestModel;

namespace CodeArchitects.Platform.Actors.Scheduling;

public partial class ActivityTests
{
  [Theory]
  [TaskActivityData]
  internal void ExecuteAsync_ShouldCallActorMethod_WhenMethodIsTaskMethod(Activity<StandardActor> sut, int arg)
  {
    // Arrange
    Mock<StandardActor> actorMock = new(MockBehavior.Loose);

    // Act
    sut.ExecuteAsync(actorMock.Object, CancellationToken.None);

    // Assert
    actorMock.Verify(x => x.TaskMethod(arg));
  }

  [Theory]
  [TaskTActivityData]
  internal void ExecuteAsync_ShouldCallActorMethod_WhenMethodIsTaskTMethod(Activity<StandardActor> sut, int arg)
  {
    // Arrange
    Mock<StandardActor> actorMock = new(MockBehavior.Loose);

    // Act
    sut.ExecuteAsync(actorMock.Object, CancellationToken.None);

    // Assert
    actorMock.Verify(x => x.TaskMethod(arg, CancellationToken.None));
  }

  [Theory]
  [ValueTaskActivityData]
  internal void ExecuteAsync_ShouldCallActorMethod_WhenMethodIsValueTaskMethod(Activity<StandardActor> sut)
  {
    // Arrange
    Mock<StandardActor> actorMock = new(MockBehavior.Loose);

    // Act
    sut.ExecuteAsync(actorMock.Object, CancellationToken.None);

    // Assert
    actorMock.Verify(x => x.ValueTaskMethod(CancellationToken.None));
  }

  [Theory]
  [ValueTaskTActivityData]
  internal void ExecuteAsync_ShouldCallActorMethod_WhenMethodIsValueTaskTMethod(Activity<StandardActor> sut)
  {
    // Arrange
    Mock<StandardActor> actorMock = new(MockBehavior.Loose);

    // Act
    sut.ExecuteAsync(actorMock.Object, CancellationToken.None);

    // Assert
    actorMock.Verify(x => x.ValueTaskTMethod());
  }

  [Theory]
  [VoidActivityData]
  internal void ExecuteAsync_ShouldCallActorMethod_WhenMethodIsVoidActivity(Activity<StandardActor> sut)
  {
    // Arrange
    Mock<StandardActor> actorMock = new(MockBehavior.Loose);

    // Act
    sut.ExecuteAsync(actorMock.Object, CancellationToken.None);

    // Assert
    actorMock.Verify(x => x.VoidActivity());
  }

  [Theory]
  [ActivityOverload1Data]
  internal void ExecuteAsync_ShouldCallActorMethod_WhenMethodIsActivityOverload1(Activity<StandardActor> sut, int arg)
  {
    // Arrange
    Mock<StandardActor> actorMock = new(MockBehavior.Loose);

    // Act
    sut.ExecuteAsync(actorMock.Object, CancellationToken.None);

    // Assert
    actorMock.Verify(x => x.ActivityOverload(arg));
  }

  [Theory]
  [ActivityOverload2Data]
  internal void ExecuteAsync_ShouldCallActorMethod_WhenMethodIsActivityOverload2(Activity<StandardActor> sut, string arg)
  {
    // Arrange
    Mock<StandardActor> actorMock = new(MockBehavior.Loose);

    // Act
    sut.ExecuteAsync(actorMock.Object, CancellationToken.None);

    // Assert
    actorMock.Verify(x => x.ActivityOverload(arg));
  }

  [Theory]
  [SpecificImplementationActivityData]
  internal void ExecuteAsync_ShouldCallActorMethod_WhenMethodIsSpecificImplementationActivity(Activity<PolymorphicActor> sut)
  {
    // Arrange
    Mock<PolymorphicActorImplementation1> actorMock = new(MockBehavior.Loose);

    // Act
    sut.ExecuteAsync(actorMock.Object, CancellationToken.None);

    // Assert
    actorMock.Verify(x => x.Activity());
  }
}
