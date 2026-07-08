using FluentAssertions;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

public class Utf8DocumentTests
{
  [Theory]
  [InlineData(OperationKind.Query)]
  [InlineData(OperationKind.Mutation)]
  public void CreateRequest_ShouldReturnCorrectOperationRequest(OperationKind operationKind)
  {
    // Arrange
    string key = "key";
    object variable = new();
    Dictionary<string, object?> variables = new() { [key] = variable };

    Upload file = new();
    Dictionary<string, Upload?> files = new() { [key] = file };

    string name = "name";
    byte[] content = { 1, 2, 3 };
    string id = "id";

    Utf8Document sut = new(operationKind, name, content, id);

    // Act
    OperationRequest request = sut.CreateRequest(variables, files, RequestStrategy.Default);

    // Assert
    request.Name.Should().Be(name);
    request.Id.Should().Be(id);
    request.Document.Kind.Should().Be(operationKind);
    request.Document.Body.ToArray().Should().BeEquivalentTo(content);
    request.Variables.Should().ContainKey(key).WhoseValue.Should().Be(variable);
    request.Files.Should().ContainKey(key).WhoseValue.Should().Be(file);
  }

  [Theory]
  [InlineData(OperationKind.Query)]
  [InlineData(OperationKind.Mutation)]
  public void CreateRequestWithoutVariables_ShouldReturnCorrectOperationRequest(OperationKind operationKind)
  {
    // Arrange
    string name = "name";
    byte[] content = { 1, 2, 3 };
    string id = "id";

    Utf8Document sut = new(operationKind, name, content, id);

    // Act
    OperationRequest request = sut.CreateRequest(RequestStrategy.Default);

    // Assert
    request.Name.Should().Be(name);
    request.Id.Should().Be(id);
    request.Document.Kind.Should().Be(operationKind);
    request.Document.Body.ToArray().Should().BeEquivalentTo(content);
    request.Variables.Should().BeEmpty();
    request.Files.Should().BeEmpty();
  }
}
