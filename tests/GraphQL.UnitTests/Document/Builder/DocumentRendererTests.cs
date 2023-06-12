using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Nodes.FluentMock;
using CodeArchitects.Platform.GraphQL.Fixtures;
using CodeArchitects.Platform.GraphQL.Fixtures.Model;
using CodeArchitects.Platform.GraphQL.Model;
using CodeArchitects.Platform.GraphQL.Model.FluentMock;
using CodeArchitects.Platform.GraphQL.UnitTests.FluentMock;
using FluentAssertions;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public class DocumentRendererTests : IDisposable
{
  private const string s_queryName = "GetBlogs";

  private readonly TestBufferWriter _writer;
  private readonly DocumentBuilderOptions _options;

  public DocumentRendererTests()
  {
    _writer = new();
    _options = new();
  }

  private string Content => _writer.ToString();

  private DocumentRenderer CreateSut() => new DocumentRenderer(_writer, _options);

  [Fact]
  public void QueryBlogFieldWithoutVariables_ShouldProduceCorrectDocument()
  {
    // Arrange
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias(null)
            .SetFieldName("blogs")
            .SetDirectives()
            .SetArguments()
            .SetSelectionSet(null as ISelectionSetNode)))));

    DocumentRenderer sut = CreateSut();
    
    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("""
      query GetBlogs {
        blogs
      }
      """);
  }

  [Fact]
  public void QueryBlogFieldWithVariables_ShouldProduceCorrectDocument()
  {
    // Arrange
    IScalarType integerType = ScalarTypeBuilder.Build(_ => _
      .SetName("Integer")
      .SetClrType(typeof(int))
      .SetKind(TypeKind.Scalar));

    IScalarType stringType = ScalarTypeBuilder.Build(_ => _
      .SetName("String")
      .SetClrType(typeof(string))
      .SetKind(TypeKind.Scalar));

    IReadOnlyList<IVariable> variables = ListBuilder<IVariable>.Build(_ => _
      .Add(VariableBuilder.Build(_ => _
        .SetName(nameof(GetBlogsVariables.First))
        .SetType(integerType)))
      .Add(VariableBuilder.Build(_ => _
        .SetName(nameof(GetBlogsVariables.Last))
        .SetType(integerType)))
      .Add(VariableBuilder.Build(_ => _
        .SetName(nameof(GetBlogsVariables.Before))
        .SetType(stringType)))
      .Add(VariableBuilder.Build(_ => _
        .SetName(nameof(GetBlogsVariables.After))
        .SetType(stringType))));

    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariables(variables)
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias(null)
            .SetFieldName("blogs")
            .SetDirectives()
            .SetArguments()
            .SetSelectionSet(null as ISelectionSetNode)))));

    DocumentRenderer sut = CreateSut();
    
    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("""
      query GetBlogs($first: Integer, $last: Integer, $before: String, $after: String) {
        blogs
      }
      """);
  }

  [Theory]
  [InlineData(1, "1")]
  [InlineData(1.5, "1.5")]
  [InlineData(1.5f, "1.5")]
  [InlineData("literal-string", "\"literal-string\"")]
  [InlineData(true, "true")]
  [InlineData(false, "false")]
  [InlineData(null, "null")]
  public void QueryBlogFieldWithLiteralArgument_ShouldProduceCorrectDocument(object? value, string valueString)
  {
    // Arrange
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias(null)
            .SetFieldName("blogs")
            .SetDirectives()
            .SetArguments(_ => _
              .Add(_ => _
                .SetName("literalArg")
                .SetValue(value)))
            .SetSelectionSet(null as ISelectionSetNode)))));

    DocumentRenderer sut = CreateSut();
    
    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be($$"""
      query GetBlogs {
        blogs(literalArg: {{valueString}})
      }
      """);
  }

  [Fact]
  public void QueryBlogFieldWithVariableArgument_ShouldProduceCorrectDocument()
  {
    // Arrange
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias(null)
            .SetFieldName("blogs")
            .SetDirectives()
            .SetArguments(_ => _
              .Add(_ => _
                .SetName("first")
                .SetValue(typeof(GetBlogsVariables).GetRequiredProperty(nameof(GetBlogsVariables.First))))
              .Add(_ => _
                .SetName("Last")
                .SetValue(typeof(GetBlogsVariables).GetRequiredProperty(nameof(GetBlogsVariables.Last)))))
            .SetSelectionSet(null as ISelectionSetNode)))));

    DocumentRenderer sut = CreateSut();
    
    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("""
      query GetBlogs {
        blogs(first: $first, last: $last)
      }
      """);
  }

  [Fact]
  public void QueryBlogFieldWithObjectArgument_ShouldProduceCorrectDocument()
  {
    // Arrange
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias(null)
            .SetFieldName("blogs")
            .SetDirectives()
            .SetArguments(_ => _
              .Add(_ => _
                .SetName("obj")
                .SetValue(ObjectValueNodeBuilder.Build(_ => _
                  .SetFields(_ => _
                    .Add(_ => _
                      .SetName("ScalarField")
                      .SetValue(1))
                    .Add(_ => _
                      .SetName("ObjectField")
                      .SetValue(ObjectValueNodeBuilder.Build(_ => _
                        .SetFields(_ => _
                          .Add(_ => _
                            .SetName("InnerField1")
                            .SetValue("inner-field-1"))
                          .Add(_ => _
                            .SetName("InnerField2")
                            .SetValue(2))))))
                    .Add(_ => _
                      .SetName("ListField")
                      .SetValue(ListValueNodeBuilder.Build(_ => _
                        .SetValues(new object?[] { 1, 2, 3 })))))))))
            .SetSelectionSet(null as ISelectionSetNode)))));

    DocumentRenderer sut = CreateSut();
    
    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("""
      query GetBlogs {
        blogs(obj: { scalarField: 1, objectField: { innerField1: "inner-field-1", innerField2: 2 }, listField: [1, 2, 3] })
      }
      """);
  }

  [Fact]
  public void QueryBlogFieldWithDirectives_ShouldProduceCorrectDocument()
  {
    // Arrange
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariables()
      .SetDirectives(_ => _
        .Add(_ => _
          .SetName("directive1")
          .SetArguments())
        .Add(_ => _
          .SetName("directive2")
          .SetArguments(_ => _
            .Add(_ => _
              .SetName("directiveArg")
              .SetValue(1)))))
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias(null)
            .SetFieldName("blogs")
            .SetDirectives()
            .SetArguments()
            .SetSelectionSet(null as ISelectionSetNode)))));

    DocumentRenderer sut = CreateSut();
    
    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("""
      query GetBlogs @directive1 @directive2(directiveArg: 1) {
        blogs
      }
      """);
  }

  [Fact]
  public void QueryBlogFieldWithAlias_ShouldProduceCorrectDocument()
  {
    // Arrange
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias("blogs")
            .SetFieldName("blogsConnection")
            .SetDirectives()
            .SetArguments()
            .SetSelectionSet(null as ISelectionSetNode)))));

    DocumentRenderer sut = CreateSut();
    
    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("""
      query GetBlogs {
        blogs: blogsConnection
      }
      """);
  }

  [Fact]
  public void QueryBlogFieldWithSelection_ShouldProduceCorrectDocument()
  {
    // Arrange
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias(null)
            .SetFieldName("blogs")
            .SetDirectives()
            .SetArguments()
            .SetSelectionSet(_ => _
              .SetSelections(_ => _
                .Add(_ => _
                  .SetAlias(null)
                  .SetFieldName("Edges")
                  .SetDirectives()
                  .SetArguments()
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add(_ => _
                        .SetAlias(null)
                        .SetFieldName("Cursor")
                        .SetDirectives()
                        .SetArguments()
                        .SetSelectionSet(null as ISelectionSetNode)))))
                .Add(_ => _
                  .SetAlias(null)
                  .SetFieldName("PageInfo")
                  .SetDirectives()
                  .SetArguments()
                  .SetSelectionSet(null as ISelectionSetNode))))))));

    DocumentRenderer sut = CreateSut();
    
    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("""
      query GetBlogs {
        blogs {
          edges {
            cursor
          }
          pageInfo
        }
      }
      """);
  }

  [Fact]
  public void QueryBlogFieldWithSelection_ShouldProduceCorrectDocument_WhenNoIndentation()
  {
    // Arrange
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias(null)
            .SetFieldName("blogs")
            .SetDirectives()
            .SetArguments()
            .SetSelectionSet(_ => _
              .SetSelections(_ => _
                .Add(_ => _
                  .SetAlias(null)
                  .SetFieldName("Edges")
                  .SetDirectives()
                  .SetArguments()
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add(_ => _
                        .SetAlias(null)
                        .SetFieldName("Cursor")
                        .SetDirectives()
                        .SetArguments()
                        .SetSelectionSet(null as ISelectionSetNode)))))
                .Add(_ => _
                  .SetAlias(null)
                  .SetFieldName("PageInfo")
                  .SetDirectives()
                  .SetArguments()
                  .SetSelectionSet(null as ISelectionSetNode))))))));

    _options.LinePolicy = LinePolicy.Space(1);

    DocumentRenderer sut = CreateSut();
    
    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("query GetBlogs { blogs { edges { cursor } pageInfo } }");
  }

  public void Dispose()
  {
    ((IDisposable)_writer).Dispose();
  }
}
