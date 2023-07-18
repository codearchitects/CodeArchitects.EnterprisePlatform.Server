using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using FluentAssertions;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

public class ExpressionGraphDocumentTests
{
  private static readonly string s_documentName = "name";
  private static readonly string s_variableName = "variable";
  private static readonly Expression s_expression = Expression.Constant(null);

  private readonly Mock<IGraphDocumentContext> _contextMock;

  public ExpressionGraphDocumentTests()
  {
    _contextMock = new(MockBehavior.Strict);
    _contextMock
      .Setup(x => x.GetService<INodeContext>())
      .Returns(Mock.Of<INodeContext>());
    
    _contextMock
      .Setup(x => x.Model.GetVariables(typeof(object)))
      .Returns(new[] { Mock.Of<IVariable>(variable => variable.Name == s_variableName, MockBehavior.Strict) });
  }

  [Fact]
  public void CreateOperationDefinition_ShouldReturnCorrectOperationDefinition_WhenGraphDocumentIsQueryWithNoVariables()
  {
    // Arrange
    var sut = new ExpressionGraphDocument<object>.Query(s_documentName, s_expression);

    // Act
    IOperationDefinitionNode operationDefinition = sut.CreateOperationDefinition(_contextMock.Object);

    // Assert
    operationDefinition.OperationType.Should().Be(OperationType.Query);
    operationDefinition.Name.ToString().Should().Be(s_documentName);
    operationDefinition.VariableDefinitionList.Should().BeNull();
  }

  [Fact]
  public void CreateOperationDefinition_ShouldReturnCorrectOperationDefinition_WhenGraphDocumentIsMutationWithNoVariables()
  {
    // Arrange
    var sut = new ExpressionGraphDocument<object>.Mutation(s_documentName, s_expression);

    // Act
    IOperationDefinitionNode operationDefinition = sut.CreateOperationDefinition(_contextMock.Object);

    // Assert
    operationDefinition.OperationType.Should().Be(OperationType.Mutation);
    operationDefinition.Name.ToString().Should().Be(s_documentName);
    operationDefinition.VariableDefinitionList.Should().BeNull();
  }

  [Fact]
  public void CreateOperationDefinition_ShouldReturnCorrectOperationDefinition_WhenGraphDocumentIsQueryWithVariables()
  {
    // Arrange
    var sut = new ExpressionGraphDocument<object, object>.Query(s_documentName, s_expression);

    // Act
    IOperationDefinitionNode operationDefinition = sut.CreateOperationDefinition(_contextMock.Object);

    // Assert
    operationDefinition.OperationType.Should().Be(OperationType.Query);
    operationDefinition.Name.ToString().Should().Be(s_documentName);
    operationDefinition.VariableDefinitionList.Should().NotBeNull();
    operationDefinition.VariableDefinitionList!.VariableDefinitions.Should().HaveCount(1);
  }

  [Fact]
  public void CreateOperationDefinition_ShouldReturnCorrectOperationDefinition_WhenGraphDocumentIsMutationWithVariables()
  {
    // Arrange
    var sut = new ExpressionGraphDocument<object, object>.Mutation(s_documentName, s_expression);

    // Act
    IOperationDefinitionNode operationDefinition = sut.CreateOperationDefinition(_contextMock.Object);

    // Assert
    operationDefinition.OperationType.Should().Be(OperationType.Mutation);
    operationDefinition.Name.ToString().Should().Be(s_documentName);
    operationDefinition.VariableDefinitionList.Should().NotBeNull();
    operationDefinition.VariableDefinitionList!.VariableDefinitions.Should().HaveCount(1);
  }
}
