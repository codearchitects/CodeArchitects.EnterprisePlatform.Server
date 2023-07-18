using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Nodes.FluentMock;
using FluentAssertions;
using System.Buffers;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

public class DocumentRendererTests
{
  private const string s_queryName = "GetBlogs";

  private readonly ArrayBufferWriter<byte> _writer;
  private readonly DocumentSerializationOptions _options;

  public DocumentRendererTests()
  {
    _writer = new();
    _options = new();
  }

  private string? Content => Encoding.UTF8.GetString(_writer.WrittenSpan);

  private DocumentRenderer CreateSut() => new DocumentRenderer(_writer, _options);

  [Fact]
  public void QueryBlogFieldWithoutVariables_ShouldProduceCorrectDocument()
  {
    // Arrange
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("")
            .SetFieldName("blogs")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(null as IArgumentListNode)
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
    IOperationDefinitionNode operationDefinition = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariableDefinitionList(_ => _
        .SetVariableDefinitions(_ => _
          .Add(_ => _
            .SetVariable(_ => _
              .SetName("arg1"))
            .SetType<INamedTypeNode>(_ => _
              .SetTypeKind(TypeNodeKind.NamedType)
              .SetName("Integer")))
          .Add(_ => _
            .SetVariable(_ => _
              .SetName("arg2"))
            .SetType<INonNullTypeNode>(_ => _
              .SetTypeKind(TypeNodeKind.NonNullType)
              .SetNullableType<INamedTypeNode>(_ => _
                .SetTypeKind(TypeNodeKind.NamedType)
                .SetName("ID"))))
          .Add(_ => _
            .SetVariable(_ => _
              .SetName("arg3"))
            .SetType<IListTypeNode>(_ => _
              .SetTypeKind(TypeNodeKind.ListType)
              .SetItemType<INamedTypeNode>(_ => _
                .SetTypeKind(TypeNodeKind.NamedType)
                .SetName("String"))))))
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("")
            .SetFieldName("blogs")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(null as IArgumentListNode)
            .SetSelectionSet(null as ISelectionSetNode)))));
  
    DocumentRenderer sut = CreateSut();
  
    // Act
    sut.AppendOperationDefinition(operationDefinition);
  
    // Assert
    Content.Should().Be("""
      query GetBlogs($arg1: Integer, $arg2: ID!, $arg3: [String]) {
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
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("")
            .SetFieldName("blogs")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(_ => _
              .SetArguments(_ => _
                .Add(_ => _
                  .SetName("literalArg")
                  .SetValue(value))))
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
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("")
            .SetFieldName("blogs")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(_ => _
              .SetArguments(_ => _
                .Add(_ => _
                  .SetName("first")
                  .SetValue(VariableNodeBuilder.Build(_ => _
                    .SetName("arg1"))))
                .Add(_ => _
                  .SetName("last")
                  .SetValue(VariableNodeBuilder.Build(_ => _
                    .SetName("arg2"))))))
            .SetSelectionSet(null as ISelectionSetNode)))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("""
      query GetBlogs {
        blogs(first: $arg1, last: $arg2)
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
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("")
            .SetFieldName("blogs")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(_ => _
              .SetArguments(_ => _
                .Add(_ => _
                  .SetName("obj")
                  .SetValue(ObjectValueNodeBuilder.Build(_ => _
                    .SetFields(_ => _
                      .Add(_ => _
                        .SetName("scalarField")
                        .SetValue(1))
                      .Add(_ => _
                        .SetName("objectField")
                        .SetValue(ObjectValueNodeBuilder.Build(_ => _
                          .SetFields(_ => _
                            .Add(_ => _
                              .SetName("innerField1")
                              .SetValue("inner-field-1"))
                            .Add(_ => _
                              .SetName("innerField2")
                              .SetValue(2))))))
                      .Add(_ => _
                        .SetName("listField")
                        .SetValue(ListValueNodeBuilder.Build(_ => _
                          .SetValues(new object?[] { 1, 2, 3 }))))))))))
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
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(_ => _
        .SetDirectives(_ => _
          .Add(_ => _
            .SetName("directive1")
            .SetArgumentList(null as IArgumentListNode))
          .Add(_ => _
            .SetName("directive2")
            .SetArgumentList(_ => _
              .SetArguments(_ => _
                .Add(_ => _
                  .SetName("directiveArg")
                  .SetValue(1)))))))
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("")
            .SetFieldName("blogs")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(null as IArgumentListNode)
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
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("blogs")
            .SetFieldName("blogsConnection")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(null as IArgumentListNode)
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
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("")
            .SetFieldName("blogs")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(null as IArgumentListNode)
            .SetSelectionSet(_ => _
              .SetSelections(_ => _
                .Add<FieldNodeBuilder>(_ => _
                  .SetSelectionKind(SelectionNodeKind.Field)
                  .SetAlias("")
                  .SetFieldName("edges")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add<FieldNodeBuilder>(_ => _
                        .SetSelectionKind(SelectionNodeKind.Field)
                        .SetAlias("")
                        .SetFieldName("cursor")
                        .SetDirectiveList(null as IDirectiveListNode)
                        .SetArgumentList(null as IArgumentListNode)
                        .SetSelectionSet(null as ISelectionSetNode)))))
                .Add<FieldNodeBuilder>(_ => _
                  .SetSelectionKind(SelectionNodeKind.Field)
                  .SetAlias("")
                  .SetFieldName("pageInfo")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
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
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("")
            .SetFieldName("blogs")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(null as IArgumentListNode)
            .SetSelectionSet(_ => _
              .SetSelections(_ => _
                .Add<FieldNodeBuilder>(_ => _
                  .SetSelectionKind(SelectionNodeKind.Field)
                  .SetAlias("")
                  .SetFieldName("edges")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add<FieldNodeBuilder>(_ => _
                        .SetSelectionKind(SelectionNodeKind.Field)
                        .SetAlias("")
                        .SetFieldName("cursor")
                        .SetDirectiveList(null as IDirectiveListNode)
                        .SetArgumentList(null as IArgumentListNode)
                        .SetSelectionSet(null as ISelectionSetNode)))))
                .Add<FieldNodeBuilder>(_ => _
                  .SetSelectionKind(SelectionNodeKind.Field)
                  .SetAlias("")
                  .SetFieldName("pageInfo")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(null as ISelectionSetNode))))))));

    _options.LinePolicy = LinePolicy.Space(1);

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendOperationDefinition(operationDefinition);

    // Assert
    Content.Should().Be("query GetBlogs { blogs { edges { cursor } pageInfo } }");
  }
}
