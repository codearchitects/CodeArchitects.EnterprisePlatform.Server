using CodeArchitects.Platform.Actors.Dapr.Fixtures;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
using Dapr.Actors.Runtime;
using FluentAssertions;
using Moq;
using System.Text.Json;

namespace CodeArchitects.Platform.Actors.Dapr.Infrastructure;

public class DaprActorHostTests
{
  [Fact]
  public void ActorId_ShouldReturnCorrectId()
  {
    // Arrange
    string id = "myId";
    Mock<IActorManager<TestActor, TestActorState>> managerMock = new(MockBehavior.Strict);
    Mock<IImplementationFactory<TestActor, TestActorState>> factoryMock = new(MockBehavior.Strict);

    ActorHost host = ActorHost.CreateForTest<TestActorHost>(new ActorTestOptions
    {
      ActorId = new(id)
    });

    TestActorHost sut = new(host, managerMock.Object, factoryMock.Object);

    // Act
    string actorId = sut.ActorId;

    // Assert
    actorId.Should().Be(id);
  }

  [Fact]
  public async Task ScheduleAsync_ShouldCallRegisterReminderAsync()
  {
    // Arrange
    int arg = 12;
    string id = "myId";
    byte[] payload = """{"$activity":1,"arg":12}"""u8.ToArray();

    JsonSerializerOptions jsonSerializerOptions = new()
    {
      TypeInfoResolver = new TestActorActivityTypeResolver(),
      IgnoreReadOnlyProperties = true
    };

    Mock<ActorTimerManager> timerManagerMock = new(MockBehavior.Strict);
    Mock<IActorManager<TestActor, TestActorState>> managerMock = new(MockBehavior.Strict);
    Mock<IImplementationFactory<TestActor, TestActorState>> factoryMock = new(MockBehavior.Strict);

    timerManagerMock
      .Setup(x => x.RegisterReminderAsync(It.IsAny<ActorReminder>()))
      .Returns(Task.CompletedTask);

    managerMock
      .Setup(x => x.ActivityType)
      .Returns(typeof(TestActorActivity));
    managerMock
      .Setup(x => x.JsonSerializerOptions)
      .Returns(jsonSerializerOptions);

    ScheduleId scheduleId = ScheduleId.New();
    TimeSpan timer = TimeSpan.FromSeconds(1);
    TimeSpan period = TimeSpan.FromSeconds(2);

    ActorHost host = ActorHost.CreateForTest<TestActorHost>(new ActorTestOptions
    {
      ActorId = new(id),
      TimerManager = timerManagerMock.Object
    });

    TestActorHost sut = new(host, managerMock.Object, factoryMock.Object);

    // Act
    await sut.ScheduleAsync(new TestActorActivity1 { arg = arg }, new SchedulingOptions(scheduleId, timer, period), CancellationToken.None);

    // Assert
    timerManagerMock.Verify(x => x.RegisterReminderAsync(It.Is<ActorReminder>(reminder =>
      reminder.Name == scheduleId.Id &&
      reminder.DueTime == timer &&
      reminder.Period == period &&
      reminder.State.SequenceEqual(payload))));
  }

  [Fact]
  public async Task UnscheduleAsync_ShouldCallUnregisterReminderAsync()
  {
    // Arrange
    string id = "myId";

    Mock<ActorTimerManager> timerManagerMock = new(MockBehavior.Strict);
    Mock<IActorManager<TestActor, TestActorState>> managerMock = new(MockBehavior.Strict);
    Mock<IImplementationFactory<TestActor, TestActorState>> factoryMock = new(MockBehavior.Strict);

    timerManagerMock
      .Setup(x => x.UnregisterReminderAsync(It.IsAny<ActorReminderToken>()))
      .Returns(Task.CompletedTask);

    ScheduleId scheduleId = ScheduleId.New();

    ActorHost host = ActorHost.CreateForTest<TestActorHost>(new ActorTestOptions
    {
      ActorId = new(id),
      TimerManager = timerManagerMock.Object
    });

    TestActorHost sut = new(host, managerMock.Object, factoryMock.Object);

    // Act
    await sut.UnscheduleAsync(scheduleId, CancellationToken.None);

    // Assert
    timerManagerMock.Verify(x => x.UnregisterReminderAsync(It.Is<ActorReminderToken>(reminder => reminder.Name == scheduleId.Id)));
  }

  [Fact]
  public async Task ReceiveReminderAsync_ShouldInvokeActivity()
  {
    // Arrange
    int arg = 12;
    string id = "myId";
    byte[] payload = """{"$activity":1,"arg":12}"""u8.ToArray();

    JsonSerializerOptions jsonSerializerOptions = new()
    {
      TypeInfoResolver = new TestActorActivityTypeResolver(),
      IgnoreReadOnlyProperties = true
    };

    Mock<IActorStateManager> stateManagerMock = new(MockBehavior.Strict);
    Mock<IActorManager<TestActor, TestActorState>> managerMock = new(MockBehavior.Strict);
    Mock<IImplementationFactory<TestActor, TestActorState>> factoryMock = new(MockBehavior.Strict);
    Mock<TestActor> actorMock = new(MockBehavior.Loose);

    stateManagerMock
      .Setup(x => x.TryGetStateAsync<TestActorState>(Constants.ActorStateName, It.IsAny<CancellationToken>()))
      .ReturnsAsync(new ConditionalValue<TestActorState>(true, new TestActorState()));

    managerMock
      .Setup(x => x.ActivityType)
      .Returns(typeof(TestActorActivity));
    managerMock
      .Setup(x => x.JsonSerializerOptions)
      .Returns(jsonSerializerOptions);

    factoryMock
      .Setup(x => x.Create(It.IsAny<IActorHost<TestActor, TestActorState>>(), It.IsAny<TestActorState>(), It.IsAny<int>()))
      .Returns(actorMock.Object);

    ActorHost host = ActorHost.CreateForTest<TestActorHost>(new ActorTestOptions
    {
      ActorId = new(id)
    });

    TestActorHost sut = new(host, managerMock.Object, factoryMock.Object);
    sut.SetStateManager(stateManagerMock.Object);

    // Act
    await sut.ReceiveReminderAsync("", payload, TimeSpan.Zero, TimeSpan.Zero);

    // Assert
    actorMock.Verify(x => x.Activity(arg, It.IsAny<CancellationToken>()));
  }
}
