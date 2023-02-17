using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Actors.TestModel;
using Dapr.Actors.Runtime;
using System.Text;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

public partial class DaprActorHostTests
{
  [Theory]
  [HostData]
  internal void ActorId_ShouldReturnCorrectId(
    Mock<ActorTimerManager> timerManagerMock,
    Mock<IActorStateManager> stateManagerMock,
    Mock<IActorManager<StandardActor, StandardActorState>> managerMock,
    Mock<IManagerFactory<StandardActor, StandardActorState>> factoryMock,
    DaprActorHost<StandardActor, StandardActorState> sut)
  {
    // Arrange

    // Act
    string actorId = sut.ActorId;
    
    // Assert
    actorId.Should().Be(HostDataAttribute.Id);
  }

  [Theory]
  [HostData]
  internal async Task ScheduleAsync_ShouldCallRegisterReminderAsync(
    Mock<ActorTimerManager> timerManagerMock,
    Mock<IActorStateManager> stateManagerMock,
    Mock<IActorManager<StandardActor, StandardActorState>> managerMock,
    Mock<IManagerFactory<StandardActor, StandardActorState>> factoryMock,
    DaprActorHost<StandardActor, StandardActorState> sut)
  {
    // Arrange
    int arg = 12;
    byte[] payload = Encoding.UTF8.GetBytes($$"""{":id":1,"arg":{{arg}}}""");

    timerManagerMock
      .Setup(x => x.RegisterReminderAsync(It.IsAny<ActorReminder>()))
      .Returns(Task.CompletedTask);

    managerMock
      .Setup(x => x.ActivityType)
      .Returns(typeof(StandardActorActivity));
    managerMock
      .Setup(x => x.JsonSerializerOptions)
      .Returns(StandardActorFixture.Descriptor.JsonSerializerOptions);

    ScheduleId scheduleId = ScheduleId.New();
    TimeSpan timer = TimeSpan.FromSeconds(1);
    TimeSpan period = TimeSpan.FromSeconds(2);

    // Act
    await sut.ScheduleAsync(new StandardActorActivity1 { arg = arg }, new SchedulingOptions(scheduleId, timer, period), CancellationToken.None);

    // Assert
    timerManagerMock.Verify(x => x.RegisterReminderAsync(It.Is<ActorReminder>(reminder =>
      reminder.Name == scheduleId.Id &&
      reminder.DueTime == timer &&
      reminder.Period == period &&
      reminder.State.SequenceEqual(payload))));
  }

  [Theory]
  [HostData]
  internal async Task UnscheduleAsync_ShouldCallUnregisterReminderAsync(
    Mock<ActorTimerManager> timerManagerMock,
    Mock<IActorStateManager> stateManagerMock,
    Mock<IActorManager<StandardActor, StandardActorState>> managerMock,
    Mock<IManagerFactory<StandardActor, StandardActorState>> factoryMock,
    DaprActorHost<StandardActor, StandardActorState> sut)
  {
    // Arrange

    timerManagerMock
      .Setup(x => x.UnregisterReminderAsync(It.IsAny<ActorReminderToken>()))
      .Returns(Task.CompletedTask);

    ScheduleId scheduleId = ScheduleId.New();

    // Act
    await sut.UnscheduleAsync(scheduleId, CancellationToken.None);

    // Assert
    timerManagerMock.Verify(x => x.UnregisterReminderAsync(It.Is<ActorReminderToken>(reminder => reminder.Name == scheduleId.Id)));
  }

  [Theory]
  [HostData]
  internal async Task ReceiveReminderAsync_ShouldInvokeActivity(
    Mock<ActorTimerManager> timerManagerMock,
    Mock<IActorStateManager> stateManagerMock,
    Mock<IActorManager<StandardActor, StandardActorState>> managerMock,
    Mock<IManagerFactory<StandardActor, StandardActorState>> factoryMock,
    DaprActorHost<StandardActor, StandardActorState> sut)
  {
    // Arrange
    int arg = 12;
    byte[] payload = Encoding.UTF8.GetBytes($$"""{":id":2,"arg":{{arg}}}""");

    Mock<StandardActor> actorMock = new(MockBehavior.Loose);

    stateManagerMock
      .Setup(x => x.TryGetStateAsync<StandardActorState>(Constants.ActorStateName, It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ConditionalValue<StandardActorState>(true, new StandardActorState()));

    managerMock
      .Setup(x => x.ActivityType)
      .Returns(typeof(StandardActorActivity));
    managerMock
      .Setup(x => x.JsonSerializerOptions)
      .Returns(StandardActorFixture.Descriptor.JsonSerializerOptions);
    factoryMock
      .Setup(x => x.Create(It.IsAny<IActorHost<StandardActor, StandardActorState>>(), It.IsAny<StandardActorState>(), It.IsAny<int>()))
      .Returns(managerMock.Object);

    managerMock
      .Setup(x => x.Actor)
      .Returns(actorMock.Object);
    managerMock
      .Setup(x => x.OnActivityBegin());

    // Act
    await sut.ReceiveReminderAsync("", payload, TimeSpan.Zero, TimeSpan.Zero);

    // Assert
    actorMock.Verify(x => x.TaskMethod(arg, It.IsAny<CancellationToken>()));
  }
}
