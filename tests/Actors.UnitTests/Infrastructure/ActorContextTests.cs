using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Metadata;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Infrastructure;

public partial class ActorContextTests
{
  private readonly Mock<IActorDescriptor<TestActor1, TestActor1State>> _descriptor1Mock;
  private readonly Mock<IActivityManager<TestActor1>> _activityManager1Mock;
  private readonly Mock<IActorHost<TestActor1>> _host1Mock;
  private readonly TestActor1State _state1;
  private readonly ActorContext<TestActor1, TestActor1State> _sut1;

  private readonly Mock<IActorDescriptor<TestActor2, TestActor2State>> _descriptor2Mock;
  private readonly Mock<IActivityManager<TestActor2>> _activityManager2Mock;
  private readonly Mock<IActorHost<TestActor2>> _host2Mock;
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

    _descriptor2Mock
      .Setup(x => x.CreateInstance(It.IsAny<int>(), It.IsAny<IServiceProvider>(), It.IsAny<TestActor2State>(), It.IsAny<IActorContext<TestActor2>>()))
      .Returns(delegate (int implementationId, IServiceProvider services, TestActor2State state, IActorContext<TestActor2> context)
      {
        return new TestActor2(state, context);
      });

    _sut1 = new(
      Mock.Of<IServiceProvider>(MockBehavior.Strict),
      _descriptor1Mock.Object,
      _activityManager1Mock.Object,
      _host1Mock.Object,
      _state1,
      0);

    _sut2 = new(
      Mock.Of<IServiceProvider>(MockBehavior.Strict),
      _descriptor2Mock.Object,
      _activityManager2Mock.Object,
      _host2Mock.Object,
      _state2,
      0);
  }

  [Fact]
  public void Become_ShouldSetImplementationIdOnState()
  {
    // Arrange
    int implementationId = 12;

    _descriptor1Mock
      .Setup(x => x.GetImplementation(typeof(TestActor1Impl)).Id)
      .Returns(implementationId);

    // Act
    _sut1.Become<TestActor1Impl>();

    // Assert
    _state1.ImplementationId.Should().Be(implementationId);
  }

  [Theory]
  [InlineData(1, 2, false, true, "precondition and postcondition are verified")]
  [InlineData(3, 2, false, false, "precondition is not verified")]
  [InlineData(1, 3, false, false, "postcondition is not verified")]
  [InlineData(1, 2, true, false, "binding is disabled")]
  public async Task ExecuteBindingsAsync_ShouldExecuteEnabledBindings(int initialValue, int finalValue, bool disableBinding, bool bindingExecuted, string because)
  {
    // Arrange
    _sut2.Actor.State.Value = initialValue;
    _sut2.OnMethodBegin();
    _sut2.Actor.State.Value = finalValue;
    _state2.EnabledBindings = disableBinding ? 0 : 1;

    // Act
    await _sut2.ExecuteBindingsAsync(CancellationToken.None);

    // Assert
    _sut2.Actor.BindingExecuted.Should().Be(bindingExecuted, because);
  }

  [Theory]
  [InlineData(0, 1)]
  [InlineData(2, 4)]
  [InlineData(4, 16)]
  public void EnableBinding_ShouldSetCorrectBitInState(int index, int expectedValue)
  {
    // Arrange

    // Act
    _sut1.EnableBinding(new BindingId(index));

    // Assert
    _state1.EnabledBindings.Should().Be(expectedValue);
  }

  [Theory]
  [InlineData(0, ~1)]
  [InlineData(2, ~4)]
  [InlineData(4, ~16)]
  public void DisableBinding_ShouldResetCorrectBitInState(int index, int expectedValue)
  {
    // Arrange
    _state1.EnabledBindings = ~0;

    // Act
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
