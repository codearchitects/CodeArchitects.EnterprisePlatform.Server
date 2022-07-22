namespace CodeArchitects.Platform.Common.Utils;

public class ReflectionExtensionsTests
{
  [Fact]
  public void InvokePublicMethod_ShouldInvokePublicMethod_WhenMethodExists()
  {
    // Arrange
    const string param1 = nameof(param1);
    const string param2 = nameof(param2);
    const string expectedResult = param1 + param2;
    Stub stub = new Stub();

    // Act
    object? result = stub.InvokePublicMethod("PublicMethod", param1, param2);

    // Assert
    result.Should().Be(expectedResult);
  }

  [Fact]
  public void InvokePublicMethod_ShouldThrowMissingMethodException_WhenMethodDoesNotExist()
  {
    // Arrange
    const string param1 = nameof(param1);
    const string param2 = nameof(param2);
    Stub stub = new Stub();

    // Act
    Func<object?> act = () => stub.InvokePublicMethod("NonExistingMethod", param1, param2);

    // Assert
    act.Should().ThrowExactly<MissingMethodException>();
  }

  [Fact]
  public void InvokeNonPublicMethod_ShouldInvokePublicMethod_WhenMethodExists()
  {
    // Arrange
    const string param1 = nameof(param1);
    const string param2 = nameof(param2);
    const string expectedResult = param1 + param2;
    Stub stub = new Stub();

    // Act
    object? result = stub.InvokeNonPublicMethod("NonPublicMethod", param1, param2);

    // Assert
    result.Should().Be(expectedResult);
  }

  [Fact]
  public void InvokeNonPublicMethod_ShouldThrowMissingMethodException_WhenMethodDoesNotExist()
  {
    // Arrange
    const string param1 = nameof(param1);
    const string param2 = nameof(param2);
    Stub stub = new Stub();

    // Act
    Func<object?> act = () => stub.InvokeNonPublicMethod("NonExistingMethod", param1, param2);

    // Assert
    act.Should().ThrowExactly<MissingMethodException>();
  }

  [Fact]
  public void GetPublicProperty_ShouldGetProperty_WhenPropertyExists()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    object? result = stub.GetPublicProperty("PublicProperty");

    // Assert
    result.Should().Be("PublicPropertyValue");
  }

  [Fact]
  public void GetPublicProperty_ShouldThrowMissingMemberException_WhenPropertyDoesNotExist()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    Func<object?> act = () => stub.GetPublicProperty("NonExistingProperty");

    // Assert
    act.Should().ThrowExactly<MissingMemberException>();
  }

  [Fact]
  public void SetPublicProperty_ShouldSetProperty_WhenPropertyExists()
  {
    // Arrange
    const string newPublicPropertyValue = nameof(newPublicPropertyValue);
    Stub stub = new Stub();

    // Act
    stub.SetPublicProperty("PublicProperty", newPublicPropertyValue);

    // Assert
    stub.GetPublicProperty("PublicProperty").Should().Be(newPublicPropertyValue);
  }

  [Fact]
  public void SetPublicProperty_ShouldThrowMissingMemberException_WhenPropertyDoesNotExist()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    Action act = () => stub.SetPublicProperty("NonExistingProperty", "");

    // Assert
    act.Should().ThrowExactly<MissingMemberException>();
  }

  [Fact]
  public void GetNonPublicProperty_ShouldGetProperty_WhenPropertyExists()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    object? result = stub.GetNonPublicProperty("NonPublicProperty");

    // Assert
    result.Should().Be("NonPublicPropertyValue");
  }

  [Fact]
  public void GetNonPublicProperty_ShouldThrowMissingMemberException_WhenPropertyDoesNotExist()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    Func<object?> act = () => stub.GetNonPublicProperty("NonExistingProperty");

    // Assert
    act.Should().ThrowExactly<MissingMemberException>();
  }

  [Fact]
  public void SetNonPublicProperty_ShouldSetProperty_WhenPropertyExists()
  {
    // Arrange
    const string newNonPublicPropertyValue = nameof(newNonPublicPropertyValue);
    Stub stub = new Stub();

    // Act
    stub.SetNonPublicProperty("NonPublicProperty", newNonPublicPropertyValue);

    // Assert
    stub.GetNonPublicProperty("NonPublicProperty").Should().Be(newNonPublicPropertyValue);
  }

  [Fact]
  public void SetNonPublicProperty_ShouldThrowMissingMemberException_WhenPropertyDoesNotExist()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    Action act = () => stub.SetNonPublicProperty("NonExistingProperty", "");

    // Assert
    act.Should().ThrowExactly<MissingMemberException>();
  }

  [Fact]
  public void GetPublicField_ShouldGetField_WhenFieldExists()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    object? result = stub.GetPublicField("PublicField");

    // Assert
    result.Should().Be("PublicFieldValue");
  }

  [Fact]
  public void GetPublicField_ShouldThrowMissingMemberException_WhenFieldDoesNotExist()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    Func<object?> act = () => stub.GetPublicField("NonExistingField");

    // Assert
    act.Should().ThrowExactly<MissingFieldException>();
  }

  [Fact]
  public void SetPublicField_ShouldSetField_WhenFieldExists()
  {
    // Arrange
    const string newPublicFieldValue = nameof(newPublicFieldValue);
    Stub stub = new Stub();

    // Act
    stub.SetPublicField("PublicField", newPublicFieldValue);

    // Assert
    stub.GetPublicField("PublicField").Should().Be(newPublicFieldValue);
  }

  [Fact]
  public void SetPublicField_ShouldThrowMissingMemberException_WhenFieldDoesNotExist()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    Action act = () => stub.SetPublicField("NonExistingField", "");

    // Assert
    act.Should().ThrowExactly<MissingFieldException>();
  }

  [Fact]
  public void GetNonPublicField_ShouldGetField_WhenFieldExists()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    object? result = stub.GetNonPublicField("NonPublicField");

    // Assert
    result.Should().Be("NonPublicFieldValue");
  }

  [Fact]
  public void GetNonPublicField_ShouldThrowMissingMemberException_WhenFieldDoesNotExist()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    Func<object?> act = () => stub.GetNonPublicField("NonExistingField");

    // Assert
    act.Should().ThrowExactly<MissingFieldException>();
  }

  [Fact]
  public void SetNonPublicField_ShouldSetField_WhenFieldExists()
  {
    // Arrange
    const string newNonPublicFieldValue = nameof(newNonPublicFieldValue);
    Stub stub = new Stub();

    // Act
    stub.SetNonPublicField("NonPublicField", newNonPublicFieldValue);

    // Assert
    stub.GetNonPublicField("NonPublicField").Should().Be(newNonPublicFieldValue);
  }

  [Fact]
  public void SetNonPublicField_ShouldThrowMissingFIeldException_WhenFieldDoesNotExist()
  {
    // Arrange
    Stub stub = new Stub();

    // Act
    Action act = () => stub.SetNonPublicField("NonExistingField", "");

    // Assert
    act.Should().ThrowExactly<MissingFieldException>();
  }

  private class Stub
  {
    public string PublicMethod(string param1, string param2) => param1 + param2;

    private string NonPublicMethod(string param1, string param2) => param1 + param2;

    public string PublicProperty { get; private set; } = "PublicPropertyValue";

    private string NonPublicProperty { get; set; } = "NonPublicPropertyValue";

    public string PublicField = "PublicFieldValue";

#pragma warning disable IDE0044 // Add readonly modifier. Reason: accessed via reflection
    private string NonPublicField = "NonPublicFieldValue";
#pragma warning restore IDE0044 // Add readonly modifier
  }
}
