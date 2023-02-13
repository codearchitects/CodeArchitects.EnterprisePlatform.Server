using CodeArchitects.Platform.Actors.TestModel;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Scheduling;

public class ActivityManagerTests
{
  [Fact]
  public void CreateActivity_ShouldReturnCorrectActivity_WithMethodInfoNoOverloads()
  {
    // Arrange
    const int implementationId = 42;
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[0].ImplementationMethod;

    // Act
    Activity<StandardActor> activity = sut.CreateActivity<StandardActor>(implementationId, method, new object?[] { arg });

    // Assert
    StandardActorActivity1 typedActivity = activity.Should().BeOfType<StandardActorActivity1>().Subject;
    typedActivity.ImplementationId.Should().Be(implementationId);
    typedActivity.arg.Should().Be(arg);
  }

  [Fact]
  public void CreateActivity_ShouldReturnCorrectActivity_WithMethodInfoCancellationToken()
  {
    // Arrange
    const int implementationId = 42;
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[1].ImplementationMethod;

    // Act
    Activity<StandardActor> activity = sut.CreateActivity<StandardActor>(implementationId, method, new object?[] { arg, CancellationToken.None });

    // Assert
    StandardActorActivity2 typedActivity = activity.Should().BeOfType<StandardActorActivity2>().Subject;
    typedActivity.ImplementationId.Should().Be(implementationId);
    typedActivity.arg.Should().Be(arg);
  }

  [Fact]
  public void CreateActivity_ShouldReturnCorrectActivity_WithMethodInfoOverload1()
  {
    // Arrange
    const int implementationId = 42;
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[5].ImplementationMethod;

    // Act
    Activity<StandardActor> activity = sut.CreateActivity<StandardActor>(implementationId, method, new object?[] { arg });

    // Assert
    StandardActorActivity6 typedActivity = activity.Should().BeOfType<StandardActorActivity6>().Subject;
    typedActivity.ImplementationId.Should().Be(implementationId);
    typedActivity.arg.Should().Be(arg);
  }

  [Fact]
  public void CreateActivity_ShouldReturnCorrectActivity_WithMethodInfoOverload2()
  {
    // Arrange
    const int implementationId = 42;
    const string arg = "12";
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[6].ImplementationMethod;

    // Act
    Activity<StandardActor> activity = sut.CreateActivity<StandardActor>(implementationId, method, new object?[] { arg });

    // Assert
    StandardActorActivity7 typedActivity = activity.Should().BeOfType<StandardActorActivity7>().Subject;
    typedActivity.ImplementationId.Should().Be(implementationId);
    typedActivity.arg.Should().Be(arg);
  }

  [Fact]
  public void CreateActivity_ShouldReturnCorrectActivity_WithActivityNameNoOverloads()
  {
    // Arrange
    const int implementationId = 42;
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[0].ImplementationMethod;

    // Act
    Activity<StandardActor> activity = sut.CreateActivity<StandardActor>(implementationId, method.Name, new object?[] { arg });

    // Assert
    StandardActorActivity1 typedActivity = activity.Should().BeOfType<StandardActorActivity1>().Subject;
    typedActivity.ImplementationId.Should().Be(implementationId);
    typedActivity.arg.Should().Be(arg);
  }

  [Fact]
  public void CreateActivity_ShouldReturnCorrectActivity_WithActivityNameCancellationToken()
  {
    // Arrange
    const int implementationId = 42;
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[1].ImplementationMethod;

    // Act
    Activity<StandardActor> activity = sut.CreateActivity<StandardActor>(implementationId, method.Name, new object?[] { arg, CancellationToken.None });

    // Assert
    StandardActorActivity2 typedActivity = activity.Should().BeOfType<StandardActorActivity2>().Subject;
    typedActivity.ImplementationId.Should().Be(implementationId);
    typedActivity.arg.Should().Be(arg);
  }

  [Fact]
  public void CreateActivity_ShouldReturnCorrectActivity_WithActivityNameOverload1()
  {
    // Arrange
    const int implementationId = 42;
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[5].ImplementationMethod;

    // Act
    Activity<StandardActor> activity = sut.CreateActivity<StandardActor>(implementationId, method.Name, new object?[] { arg });

    // Assert
    StandardActorActivity6 typedActivity = activity.Should().BeOfType<StandardActorActivity6>().Subject;
    typedActivity.ImplementationId.Should().Be(implementationId);
    typedActivity.arg.Should().Be(arg);
  }

  [Fact]
  public void CreateActivity_ShouldReturnCorrectActivity_WithActivityNameOverload2()
  {
    // Arrange
    const int implementationId = 42;
    const string arg = "12";
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[6].ImplementationMethod;

    // Act
    Activity<StandardActor> activity = sut.CreateActivity<StandardActor>(implementationId, method.Name, new object?[] { arg });

    // Assert
    StandardActorActivity7 typedActivity = activity.Should().BeOfType<StandardActorActivity7>().Subject;
    typedActivity.ImplementationId.Should().Be(implementationId);
    typedActivity.arg.Should().Be(arg);
  }
}
