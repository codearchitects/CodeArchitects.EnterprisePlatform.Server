using CodeArchitects.Platform.GraphQL.ChilliCream.Document;
using CodeArchitects.Platform.GraphQL.Document;
using FluentAssertions;
using StrawberryShake;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

public class RequestFactoryTests
{
  private readonly Mock<IOperationExecutorProvider> _executorProviderMock;
  private readonly Mock<IVariableExtractorProvider> _extractorProviderMock;
  private readonly RequestFactory _sut;

  public RequestFactoryTests()
  {
    Console.WriteLine("Hello, world"); // TODO: Delete
    _executorProviderMock = new(MockBehavior.Strict);
    _executorProviderMock
      .Setup(x => x.GetExecutor<object>())
      .Returns(Mock.Of<IOperationExecutor<object>>(MockBehavior.Strict));

    _extractorProviderMock = new(MockBehavior.Strict);
    _extractorProviderMock
      .Setup(x => x.GetExtractor<object>())
      .Returns(Mock.Of<VariableExtractor<object>>(MockBehavior.Strict));

    _sut = new(_executorProviderMock.Object, _extractorProviderMock.Object);
  }

  [Fact]
  public void CreateRequestWithoutVariables_ShouldCallCorrectMethodsOnDependencies()
  {
    // Arrange
    string name = "name";
    Utf8Document document = new(OperationKind.Query, name, Array.Empty<byte>(), "id");

    // Act
    IGraphRequest<object> request = _sut.CreateRequest<object>(OperationType.Query, name, document);

    // Assert
    request.Should().BeOfType<GraphRequest<object>>();
    _executorProviderMock.Verify(x => x.GetExecutor<object>(), Times.Once);
    _extractorProviderMock.VerifyNoOtherCalls();
  }

  [Fact]
  public void CreateRequestWithVariables_ShouldCallCorrectMethodsOnDependencies()
  {
    // Arrange
    string name = "";
    Utf8Document document = new(OperationKind.Mutation, name, Array.Empty<byte>(), "id");

    // Act
    IGraphRequest<object, object> request = _sut.CreateRequest<object, object>(OperationType.Mutation, name, document);

    // Assert
    request.Should().BeOfType<GraphRequest<object, object>>();
    _executorProviderMock.Verify(x => x.GetExecutor<object>(), Times.Once);
    _extractorProviderMock.Verify(x => x.GetExtractor<object>(), Times.Once);
  }
}
