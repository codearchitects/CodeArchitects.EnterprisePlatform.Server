using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Actors.TestModel;
using Dapr.Actors.Runtime;
using System.Text;
using static CodeArchitects.Platform.Actors.Dapr.Infrastructure.DaprActorHostFixture;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

public class DaprActorHostTests
{
  [Theory]
  [HostData]
  internal void ActorId_ShouldReturnCorrectId(DaprActorHostFixture fixture, DaprActorHost<StandardActor, StandardActorState> sut)
  {
    // Arrange
    _ = fixture; // Suppress warning

    // Act
    string actorId = sut.ActorId;
    
    // Assert
    actorId.Should().Be(ActorId);
  }

  [Theory]
  [HostData]
  internal async Task ScheduleAsync_ShouldCallRegisterReminderAsync(DaprActorHostFixture fixture, DaprActorHost<StandardActor, StandardActorState> sut)
  {
    // Arrange
    int arg = 12;
    byte[] payload = Encoding.UTF8.GetBytes($$"""{":id":1,"arg":{{arg}}}""");

    fixture.TimerManagerMock
      .Setup(x => x.RegisterReminderAsync(It.IsAny<ActorReminder>()))
      .Returns(Task.CompletedTask);

    fixture.ManagerMock
      .Setup(x => x.ActivityType)
      .Returns(typeof(StandardActorActivity));
    fixture.ManagerMock
      .Setup(x => x.JsonSerializerOptions)
      .Returns(StandardActorFixture.Descriptor.JsonSerializerOptions);

    ScheduleId scheduleId = ScheduleId.New();
    TimeSpan timer = TimeSpan.FromSeconds(1);
    TimeSpan period = TimeSpan.FromSeconds(2);

    // Act
    await sut.ScheduleAsync(scheduleId, new StandardActorActivity1 { arg = arg }, new SchedulingOptions(timer, period), CancellationToken.None);

    // Assert
    fixture.TimerManagerMock.Verify(x => x.RegisterReminderAsync(It.Is<ActorReminder>(reminder =>
      reminder.Name == scheduleId.Id &&
      reminder.DueTime == timer &&
      reminder.Period == period &&
      reminder.State.SequenceEqual(payload))));
  }

  [Theory]
  [HostData]
  internal async Task UnscheduleAsync_ShouldCallUnregisterReminderAsync(DaprActorHostFixture fixture, DaprActorHost<StandardActor, StandardActorState> sut)
  {
    // Arrange

    fixture.TimerManagerMock
      .Setup(x => x.UnregisterReminderAsync(It.IsAny<ActorReminderToken>()))
      .Returns(Task.CompletedTask);

    ScheduleId scheduleId = ScheduleId.New();

    // Act
    await sut.UnscheduleAsync(scheduleId, CancellationToken.None);

    // Assert
    fixture.TimerManagerMock.Verify(x => x.UnregisterReminderAsync(It.Is<ActorReminderToken>(reminder => reminder.Name == scheduleId.Id)));
  }

  [Theory]
  [HostData]
  internal async Task ReceiveReminderAsync_ShouldInvokeActivity(DaprActorHostFixture fixture, DaprActorHost<StandardActor, StandardActorState> sut)
  {
    // Arrange
    int arg = 12;
    byte[] payload = Encoding.UTF8.GetBytes($$"""{":id":2,"arg":{{arg}}}""");

    Mock<StandardActor> actorMock = new(MockBehavior.Loose);

    fixture.StateManagerMock
      .Setup(x => x.TryGetStateAsync<StandardActorState>(Constants.ActorStateName, It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ConditionalValue<StandardActorState>(true, new StandardActorState()));

    fixture.ManagerMock
      .Setup(x => x.ActivityType)
      .Returns(typeof(StandardActorActivity));
    fixture.ManagerMock
      .Setup(x => x.JsonSerializerOptions)
      .Returns(StandardActorFixture.Descriptor.JsonSerializerOptions);

    fixture.ManagerMock
      .Setup(x => x.Actor)
      .Returns(actorMock.Object);
    fixture.ManagerMock
      .Setup(x => x.BeginActivityAsync(It.IsAny<Activity<StandardActor>>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    // Act
    await sut.ReceiveReminderAsync("", payload, TimeSpan.Zero, TimeSpan.Zero);

    // Assert
    actorMock.Verify(x => x.TaskMethod(arg, It.IsAny<CancellationToken>()));
  }
}
