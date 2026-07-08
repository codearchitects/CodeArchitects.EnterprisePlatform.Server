using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Nodes.FluentMock;
using FluentAssertions;
using System.Buffers;
using System.Reflection;
using System.Text;
using Xunit.Sdk;

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

  private DocumentRenderer CreateSut() => new(_writer, _options);

  [Fact]
  public void QueryBlogFieldWithoutVariables_ShouldProduceCorrectDocument()
  {
    // Arrange
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                .SetSelectionSet(null as ISelectionSetNode)))))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

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
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                .SetSelectionSet(null as ISelectionSetNode)))))));
  
    DocumentRenderer sut = CreateSut();
  
    // Act
    sut.AppendDocument(document);
  
    // Assert
    Content.Should().Be("""
      query GetBlogs($arg1: Integer, $arg2: ID!, $arg3: [String]) {
        blogs
      }
      """);
  }

  [Theory]
  [LiteralValueData]
  public void QueryBlogFieldWithLiteralArgument_ShouldProduceCorrectDocument(IValueNode value, string valueString)
  {
    // Arrange
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                .SetSelectionSet(null as ISelectionSetNode)))))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

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
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                      .SetValue<IVariableNode>(_ => _
                        .SetValueKind(ValueNodeKind.Variable)
                        .SetName("arg1")))
                    .Add(_ => _
                      .SetName("last")
                      .SetValue<IVariableNode>(_ => _
                        .SetValueKind(ValueNodeKind.Variable)
                        .SetName("arg2")))))
                .SetSelectionSet(null as ISelectionSetNode)))))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

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
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                      .SetValue<IObjectValueNode>(_ => _
                        .SetValueKind(ValueNodeKind.ObjectValue)
                        .SetFields(_ => _
                          .Add(_ => _
                            .SetName("scalarField")
                            .SetValue<IIntValueNode>(_ => _
                              .SetValueKind(ValueNodeKind.IntValue)
                              .SetValue(1)))
                          .Add(_ => _
                            .SetName("objectField")
                            .SetValue<IObjectValueNode>(_ => _
                              .SetValueKind(ValueNodeKind.ObjectValue)
                              .SetFields(_ => _
                                .Add(_ => _
                                  .SetName("innerField1")
                                  .SetValue<IStringValueNode>(_ => _
                                    .SetValueKind(ValueNodeKind.StringValue)
                                    .SetValue("inner-field-1")))
                                .Add(_ => _
                                  .SetName("innerField2")
                                  .SetValue<IIntValueNode>(_ => _
                                    .SetValueKind(ValueNodeKind.IntValue)
                                    .SetValue(2))))))
                          .Add(_ => _
                            .SetName("listField")
                            .SetValue<IListValueNode>(_ => _
                              .SetValueKind(ValueNodeKind.ListValue)
                              .SetValues(_ => _
                                .Add<IntValueNodeBuilder>(_ => _
                                  .SetValueKind(ValueNodeKind.IntValue)
                                  .SetValue(1))
                                .Add<IntValueNodeBuilder>(_ => _
                                  .SetValueKind(ValueNodeKind.IntValue)
                                  .SetValue(2))
                                .Add<IntValueNodeBuilder>(_ => _
                                  .SetValueKind(ValueNodeKind.IntValue)
                                  .SetValue(3))))))))))
                .SetSelectionSet(null as ISelectionSetNode)))))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

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
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                      .SetValue<IIntValueNode>(_ => _
                        .SetValueKind(ValueNodeKind.IntValue)
                        .SetValue(1))))))))
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FieldNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.Field)
                .SetAlias("")
                .SetFieldName("blogs")
                .SetDirectiveList(null as IDirectiveListNode)
                .SetArgumentList(null as IArgumentListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

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
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                .SetSelectionSet(null as ISelectionSetNode)))))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

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
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                      .SetSelectionSet(null as ISelectionSetNode))))))))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

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
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                      .SetSelectionSet(null as ISelectionSetNode))))))))));

    _options.LinePolicy = LinePolicy.Space(1);

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

    // Assert
    Content.Should().Be("query GetBlogs { blogs { edges { cursor } pageInfo } }");
  }

  [Fact]
  public void DocumentWithOperationsAndFragments_ShouldProduceCorrectDocument()
  {
    // Arrange
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(true)
          .SetOperationType(OperationType.Query)
          .SetName("")
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
                .SetSelectionSet(null as ISelectionSetNode))
              .Add<FragmentSpreadNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.FragmentSpread)
                .SetFragmentName("myFragment")
                .SetDirectiveList(_ => _
                  .SetDirectives(_ => _
                    .Add(_ => _
                      .SetName("dir1")
                      .SetArgumentList(null as IArgumentListNode))))))))
        .Add<FragmentDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.FragmentDefinition)
          .SetFragmentName("myFragment")
          .SetTypeCondition(_ => _
            .SetType(_ => _
              .SetName("Query")))
          .SetDirectiveList(_ => _
            .SetDirectives(_ => _
              .Add(_ => _
                .SetName("dir2")
                .SetArgumentList(null as IArgumentListNode))))
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FieldNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.Field)
                .SetAlias("")
                .SetFieldName("users")
                .SetArgumentList(null as IArgumentListNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

    // Assert
    Content.Should().Be("""
      {
        blogs
        ...myFragment @dir1
      }
      fragment myFragment on Query @dir2 {
        users
      }
      """);
  }

  [Fact]
  public void DocumentWithBlockString_ShouldProduceCorrectDocument()
  {
    string messageStr = "\"\"\"\r\n    Hello,\r\n      World!\r\n    \r\n    Yours,\r\n      GraphQL.\r\n  \"\"\"";

    // Arrange
    IDocumentNode document = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Mutation)
          .SetName("")
          .SetVariableDefinitionList(null as IVariableDefinitionListNode)
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FieldNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.Field)
                .SetAlias("")
                .SetFieldName("sendEmail")
                .SetDirectiveList(null as IDirectiveListNode)
                .SetArgumentList(_ => _
                  .SetArguments(_ => _
                    .Add(_ => _
                      .SetName("message")
                      .SetValue<IBlockStringValueNode>(_ => _
                        .SetValueKind(ValueNodeKind.BlockStringValue)
                        .SetLines(_ => _
                          .Add("Hello,".AsMemory())
                          .Add("  World!".AsMemory())
                          .Add("".AsMemory())
                          .Add("Yours,".AsMemory())
                          .Add("  GraphQL.".AsMemory()))))))
                .SetSelectionSet(null as ISelectionSetNode)))))));

    DocumentRenderer sut = CreateSut();

    // Act
    sut.AppendDocument(document);

    // Assert
    Content.Should().Be($$"""
      mutation {
        sendEmail(message: {{messageStr}})
      }
      """);
  }

  private sealed class LiteralValueDataAttribute : DataAttribute
  {
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
      IIntValueNode intValue = IntValueNodeBuilder.Build(_ => _
        .SetValueKind(ValueNodeKind.IntValue)
        .SetValue(1));

      IFloatValueNode floatValue = FloatValueNodeBuilder.Build(_ => _
        .SetValueKind(ValueNodeKind.FloatValue)
        .SetValue(1.5));

      IStringValueNode stringValue = StringValueNodeBuilder.Build(_ => _
        .SetValueKind(ValueNodeKind.StringValue)
        .SetValue("literal-string"));

      IBooleanValueNode trueValue = BooleanValueNodeBuilder.Build(_ => _
        .SetValueKind(ValueNodeKind.BooleanValue)
        .SetValue(true));

      IBooleanValueNode falseValue = BooleanValueNodeBuilder.Build(_ => _
        .SetValueKind(ValueNodeKind.BooleanValue)
        .SetValue(false));

      INullValueNode nullValue = NullValueNodeBuilder.Build(_ => _
        .SetValueKind(ValueNodeKind.NullValue));

      yield return new object[] { intValue, "1" };
      yield return new object[] { floatValue, "1.5" };
      yield return new object[] { stringValue, "\"literal-string\"" };
      yield return new object[] { trueValue, "true" };
      yield return new object[] { falseValue, "false" };
      yield return new object[] { nullValue, "null" };
    }
  }
}
