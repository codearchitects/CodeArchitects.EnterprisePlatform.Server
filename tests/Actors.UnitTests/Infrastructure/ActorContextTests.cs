using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Infrastructure;

public partial class ActorContextTests
{
  private readonly Mock<IActorDescriptor<TestActor1, TestActor1State>> _descriptor1Mock;
  private readonly Mock<IActivityManager<TestActor1>> _activityManager1Mock;
  private readonly Mock<IActorHost<TestActor1, TestActor1State>> _host1Mock;
  private readonly TestActor1State _state1;
  private readonly ActorContext<TestActor1, TestActor1State> _sut1;

  private readonly Mock<IActorDescriptor<TestActor2, TestActor2State>> _descriptor2Mock;
  private readonly Mock<IActivityManager<TestActor2>> _activityManager2Mock;
  private readonly Mock<IActorHost<TestActor2, TestActor2State>> _host2Mock;
  private readonly TestActor2State _state2;
  private readonly ActorContext<TestActor2, TestActor2State> _sut2;

  public ActorContextTests()
  {
    _descriptor1Mock = new(MockBehavior.Strict);
    _activityManager1Mock = new(MockBehavior.Strict);
    _host1Mock = new(MockBehavior.Strict);
    _state1 = new TestActor1State();

    _descriptor2Mock = new(MockBehavior.Strict);
    _activityManager2Mock = new(MockBehavior.Strict);
    _host2Mock = new(MockBehavior.Strict);
    _state2 = new TestActor2State();

    _descriptor1Mock
      .Setup(x => x.CreateInstance(It.IsAny<int>(), It.IsAny<IServiceProvider>(), It.IsAny<TestActor1State>(), It.IsAny<IActorContext<TestActor1>>()))
      .Returns(new TestActor1());

    _descriptor1Mock
      .Setup(x => x.UpdateState(It.IsAny<TestActor1>(), It.IsAny<TestActor1State>()));

    _descriptor2Mock
      .Setup(x => x.CreateInstance(It.IsAny<int>(), It.IsAny<IServiceProvider>(), It.IsAny<TestActor2State>(), It.IsAny<IActorContext<TestActor2>>()))
      .Returns(delegate (int implementationId, IServiceProvider services, TestActor2State state, IActorContext<TestActor2> context)
      {
        return new TestActor2(state, context);
      });

    _descriptor2Mock
      .Setup(x => x.UpdateState(It.IsAny<TestActor2>(), It.IsAny<TestActor2State>()));

    _host1Mock
      .Setup(x => x.GetStateAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync(_state1);

    _host1Mock
      .Setup(x => x.SetStateAsync(It.IsAny<TestActor1State>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    _host2Mock
      .Setup(x => x.GetStateAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync(_state2);

    _host2Mock
      .Setup(x => x.SetStateAsync(It.IsAny<TestActor2State>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    _sut1 = new(
      Mock.Of<IServiceProvider>(MockBehavior.Strict),
      _descriptor1Mock.Object,
      _activityManager1Mock.Object,
      _host1Mock.Object);

    _sut2 = new(
      Mock.Of<IServiceProvider>(MockBehavior.Strict),
      _descriptor2Mock.Object,
      _activityManager2Mock.Object,
      _host2Mock.Object);
  }

  [Fact]
  public async Task Become_ShouldSetImplementationIdOnState()
  {
    // Arrange
    int implementationId = 12;

    _descriptor1Mock
      .Setup(x => x.GetImplementation(typeof(TestActor1Impl)).Id)
      .Returns(implementationId);

    // Act
    await _sut1.BeginMethodAsync(CancellationToken.None);
    _sut1.Become<TestActor1Impl>();

    // Assert
    _state1.ImplementationId.Should().Be(implementationId);
  }

  [Fact]
  public async Task EndMethodAsync_ShouldCleanUpBindings()
  {
    // Arrange
    await _sut2.BeginMethodAsync(CancellationToken.None);

    // Act
    await _sut2.EndMethodAsync(CancellationToken.None);
    await _sut2.BeginMethodAsync(CancellationToken.None);

    // Assert
    _sut2.Actor.BindingId._index.Should().Be(0);
  }

  [Theory]
  [InlineData(1, 2, false, true, "precondition and postcondition are verified")]
  [InlineData(3, 2, false, false, "precondition is not verified")]
  [InlineData(1, 3, false, false, "postcondition is not verified")]
  [InlineData(1, 2, true, false, "binding is disabled")]
  public async Task ExecuteBindingsAsync_ShouldExecuteEnabledBindings(int initialValue, int finalValue, bool disableBinding, bool bindingExecuted, string because)
  {
    // Arrange
    _state2.Value = initialValue;
    await _sut2.BeginMethodAsync(CancellationToken.None);
    TestActor2 actor = _sut2.Actor;

    // Act
    _state2.Value = finalValue;
    _state2.EnabledBindings = disableBinding ? 0 : 1;
    await _sut2.EndMethodAsync(CancellationToken.None);

    // Assert
    actor.BindingExecuted.Should().Be(bindingExecuted, because);
  }

  [Theory]
  [InlineData(0, 1)]
  [InlineData(2, 4)]
  [InlineData(4, 16)]
  public async Task EnableBinding_ShouldSetCorrectBitInState(int index, int expectedValue)
  {
    // Arrange

    // Act
    await _sut1.BeginMethodAsync(CancellationToken.None);
    _sut1.EnableBinding(new BindingId(index));

    // Assert
    _state1.EnabledBindings.Should().Be(expectedValue);
  }

  [Theory]
  [InlineData(0, ~1)]
  [InlineData(2, ~4)]
  [InlineData(4, ~16)]
  public async Task DisableBinding_ShouldResetCorrectBitInState(int index, int expectedValue)
  {
    // Arrange
    _state1.EnabledBindings = ~0;

    // Act
    await _sut1.BeginMethodAsync(CancellationToken.None);
    _sut1.DisableBinding(new BindingId(index));

    // Assert
    _state1.EnabledBindings.Should().Be(expectedValue);
  }

  [Fact]
  public void ScheduleAsyncWithLambda_ShouldCreateAndScheduleCorrectActivity_WhenArgumentsExplicitlyMatch()
  {
    // Arrange
    int arg = 12;
    TestActor1Activity activity = new();

    _activityManager1Mock
      .Setup(x => x.CreateActivity(It.IsAny<int>(), It.IsAny<MethodInfo>(), It.IsAny<IReadOnlyList<object?>>()))
      .Returns(activity);

    _host1Mock
      .Setup(x => x.ScheduleAsync(It.IsAny<ScheduleId>(), It.IsAny<Activity<TestActor1>>(), It.IsAny<SchedulingOptions>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    // Act
    _sut1.ScheduleAsync(actor => actor.Activity(arg));

    // Assert
    _activityManager1Mock.Verify(x => x.CreateActivity(
      0,
      It.Is<MethodInfo>(method => method.Name == nameof(Activity) && method.GetParameters()[0].ParameterType == typeof(int)),
      It.Is<IReadOnlyList<object?>>(list => list.Count == 1 && ((int)list[0]!) == arg)));
    _host1Mock.Verify(x => x.ScheduleAsync(It.IsAny<ScheduleId>(), activity, SchedulingOptions.Now, It.IsAny<CancellationToken>()));
  }

  [Fact]
  public void ScheduleAsyncWithLambda_ShouldCreateAndScheduleCorrectActivity_WhenOneArgumentIsNull()
  {
    // Arrange
    string arg = null!;
    TestActor1Activity activity = new();

    _activityManager1Mock
      .Setup(x => x.CreateActivity(It.IsAny<int>(), It.IsAny<MethodInfo>(), It.IsAny<IReadOnlyList<object?>>()))
      .Returns(activity);

    _host1Mock
      .Setup(x => x.ScheduleAsync(It.IsAny<ScheduleId>(), It.IsAny<Activity<TestActor1>>(), It.IsAny<SchedulingOptions>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    // Act
    _sut1.ScheduleAsync(actor => actor.Activity(arg));

    // Assert
    _activityManager1Mock.Verify(x => x.CreateActivity(
      0,
      It.Is<MethodInfo>(method => method.Name == nameof(Activity) && method.GetParameters()[0].ParameterType == typeof(string)),
      It.Is<IReadOnlyList<object?>>(list => list.Count == 1 && ((string)list[0]!) == arg)));
    _host1Mock.Verify(x => x.ScheduleAsync(It.IsAny<ScheduleId>(), activity, SchedulingOptions.Now, It.IsAny<CancellationToken>()));
  }

  [Fact]
  public void ScheduleAsyncWithSpec_ShouldCreateAndScheduleCorrectActivity_WhenArgumentsExplicitlyMatch()
  {
    // Arrange
    int arg = 12;
    TestActor1Activity activity = new();

    _activityManager1Mock
      .Setup(x => x.CreateActivity(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IReadOnlyList<object?>>()))
      .Returns(activity);

    _host1Mock
      .Setup(x => x.ScheduleAsync(It.IsAny<ScheduleId>(), It.IsAny<Activity<TestActor1>>(), It.IsAny<SchedulingOptions>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    // Act
    _sut1.ScheduleAsync(new ActivitySpec(nameof(TestActor1.Activity), new object?[] { arg }));

    // Assert
    _activityManager1Mock.Verify(x => x.CreateActivity(
      0,
      nameof(Activity),
      It.Is<IReadOnlyList<object?>>(list => list.Count == 1 && ((int)list[0]!) == arg)));
    _host1Mock.Verify(x => x.ScheduleAsync(It.IsAny<ScheduleId>(), activity, SchedulingOptions.Now, It.IsAny<CancellationToken>()));
  }

  [Fact]
  public void ScheduleAsyncWithSpec_ShouldCreateAndScheduleCorrectActivity_WhenOneArgumentIsNull()
  {
    // Arrange
    string arg = null!;
    TestActor1Activity activity = new();

    _activityManager1Mock
      .Setup(x => x.CreateActivity(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<IReadOnlyList<object?>>()))
      .Returns(activity);

    _host1Mock
      .Setup(x => x.ScheduleAsync(It.IsAny<ScheduleId>(), It.IsAny<Activity<TestActor1>>(), It.IsAny<SchedulingOptions>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    // Act
    _sut1.ScheduleAsync(new ActivitySpec(nameof(TestActor1.Activity), new object?[] { arg }));

    // Assert
    _activityManager1Mock.Verify(x => x.CreateActivity(
      0,
      nameof(Activity),
      It.Is<IReadOnlyList<object?>>(list => list.Count == 1 && ((string)list[0]!) == arg)));
    _host1Mock.Verify(x => x.ScheduleAsync(It.IsAny<ScheduleId>(), activity, SchedulingOptions.Now, It.IsAny<CancellationToken>()));
  }
}
