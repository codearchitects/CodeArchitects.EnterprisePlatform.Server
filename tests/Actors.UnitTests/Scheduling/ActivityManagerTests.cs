using CodeArchitects.Platform.Actors.Fixtures.Examples;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Scheduling;

public class ActivityManagerTests
{
  [Fact]
  public void CreatePayload_ShouldReturnCorrectPayload_WithMethodInfoNoOverloads()
  {
    // Arrange
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[0].ImplementationMethod;

    // Act
    ActivityPayload payload = sut.CreatePayload(method, new object?[] { arg });

    // Assert
    payload.Should().BeOfType<TaskMethodPayload>().Which.Arg.Should().Be(arg);
  }

  [Fact]
  public void CreatePayload_ShouldReturnCorrectPayload_WithMethodInfoCancellationToken()
  {
    // Arrange
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[1].ImplementationMethod;

    // Act
    ActivityPayload payload = sut.CreatePayload(method, new object?[] { arg, CancellationToken.None });

    // Assert
    payload.Should().BeOfType<TaskTMethodPayload>().Which.Arg.Should().Be(arg);
  }

  [Fact]
  public void CreatePayload_ShouldReturnCorrectPayload_WithMethodInfoOverload1()
  {
    // Arrange
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[4].ImplementationMethod;

    // Act
    ActivityPayload payload = sut.CreatePayload(method, new object?[] { arg });

    // Assert
    payload.Should().BeOfType<ActivityOverload1Payload>().Which.Arg.Should().Be(arg);
  }

  [Fact]
  public void CreatePayload_ShouldReturnCorrectPayload_WithMethodInfoOverload2()
  {
    // Arrange
    const string arg = "12";
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[5].ImplementationMethod;

    // Act
    ActivityPayload payload = sut.CreatePayload(method, new object?[] { arg });

    // Assert
    payload.Should().BeOfType<ActivityOverload2Payload>().Which.Arg.Should().Be(arg);
  }

  [Fact]
  public void CreatePayload_ShouldReturnCorrectPayload_WithActivityNameNoOverloads()
  {
    // Arrange
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[0].ImplementationMethod;

    // Act
    ActivityPayload payload = sut.CreatePayload(method.Name, new object?[] { arg });

    // Assert
    payload.Should().BeOfType<TaskMethodPayload>().Which.Arg.Should().Be(arg);
  }

  [Fact]
  public void CreatePayload_ShouldReturnCorrectPayload_WithActivityNameCancellationToken()
  {
    // Arrange
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[1].ImplementationMethod;

    // Act
    ActivityPayload payload = sut.CreatePayload(method.Name, new object?[] { arg, CancellationToken.None });

    // Assert
    payload.Should().BeOfType<TaskTMethodPayload>().Which.Arg.Should().Be(arg);
  }

  [Fact]
  public void CreatePayload_ShouldReturnCorrectPayload_WithActivityNameOverload1()
  {
    // Arrange
    const int arg = 12;
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[4].ImplementationMethod;

    // Act
    ActivityPayload payload = sut.CreatePayload(method.Name, new object?[] { arg });

    // Assert
    payload.Should().BeOfType<ActivityOverload1Payload>().Which.Arg.Should().Be(arg);
  }

  [Fact]
  public void CreatePayload_ShouldReturnCorrectPayload_WithActivityNameOverload2()
  {
    // Arrange
    const string arg = "12";
    ActivityManager sut = ActivityManager.Create(StandardActorFixture.Descriptor);

    MethodInfo method = StandardActorFixture.Descriptor.Activities[5].ImplementationMethod;

    // Act
    ActivityPayload payload = sut.CreatePayload(method.Name, new object?[] { arg });

    // Assert
    payload.Should().BeOfType<ActivityOverload2Payload>().Which.Arg.Should().Be(arg);
  }
}
