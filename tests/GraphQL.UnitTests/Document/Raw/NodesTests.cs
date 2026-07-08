using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Nodes.FluentMock;
using CodeArchitects.Platform.GraphQL.Fixtures;
using FluentAssertions;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

public class NodesTests
{
  [Fact]
  public void UnnamedQuery_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query {
        blogs
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                .SetArgumentList(null as IArgumentListNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void NamedQuery_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query GetBlogs {
        blogs
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Query)
          .SetName("GetBlogs")
          .SetVariableDefinitionList(null as IVariableDefinitionListNode)
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FieldNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.Field)
                .SetAlias("")
                .SetFieldName("blogs")
                .SetArgumentList(null as IArgumentListNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryWithVariableDefinitions_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query GetBlogs($var1: [[String!]!]!, $var2: Integer) {
        blogs
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Query)
          .SetName("GetBlogs")
          .SetVariableDefinitionList(_ => _
            .SetVariableDefinitions(_ => _
              .Add(_ => _
                .SetVariable(_ => _
                  .SetName("var1"))
                .SetType<INonNullTypeNode>(_ => _
                  .SetTypeKind(TypeNodeKind.NonNullType)
                  .SetNullableType<IListTypeNode>(_ => _
                    .SetTypeKind(TypeNodeKind.ListType)
                    .SetItemType<INonNullTypeNode>(_ => _
                      .SetTypeKind(TypeNodeKind.NonNullType)
                      .SetNullableType<IListTypeNode>(_ => _
                        .SetTypeKind(TypeNodeKind.ListType)
                        .SetItemType<INonNullTypeNode>(_ => _
                          .SetTypeKind(TypeNodeKind.NonNullType)
                          .SetNullableType<INamedTypeNode>(_ => _
                            .SetTypeKind(TypeNodeKind.NamedType)
                            .SetName("String"))))))))
              .Add(_ => _
                .SetVariable(_ => _
                  .SetName("var2"))
                .SetType<INamedTypeNode>(_ => _
                  .SetTypeKind(TypeNodeKind.NamedType)
                  .SetName("Integer")))))
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FieldNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.Field)
                .SetAlias("")
                .SetFieldName("blogs")
                .SetArgumentList(null as IArgumentListNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryWithDirectives_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query GetBlogs @dir1 @dir2(id: "my-id") {
        blogs
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Query)
          .SetName("GetBlogs")
          .SetVariableDefinitionList(null as IVariableDefinitionListNode)
          .SetDirectiveList(_ => _
            .SetDirectives(_ => _
              .Add(_ => _
                .SetName("dir1")
                .SetArgumentList(null as IArgumentListNode))
              .Add(_ => _
                .SetName("dir2")
                .SetArgumentList(_ => _
                  .SetArguments(_ => _
                    .Add(_ => _
                      .SetName("id")
                      .SetValue<IStringValueNode>(_ => _
                        .SetValueKind(ValueNodeKind.StringValue)
                        .SetValue("my-id"))))))))
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FieldNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.Field)
                .SetAlias("")
                .SetFieldName("blogs")
                .SetArgumentList(null as IArgumentListNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void FieldSelectionWithAlias_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query {
        my_blogs: blogs
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Query)
          .SetName("")
          .SetVariableDefinitionList(null as IVariableDefinitionListNode)
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FieldNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.Field)
                .SetAlias("my_blogs")
                .SetFieldName("blogs")
                .SetArgumentList(null as IArgumentListNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void FieldSelectionWithArgumentList_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query {
        blogs(id: "my-id", age: 42)
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                .SetArgumentList(_ => _
                  .SetArguments(_ => _
                    .Add(_ => _
                      .SetName("id")
                      .SetValue<IStringValueNode>(_ => _
                        .SetValueKind(ValueNodeKind.StringValue)
                        .SetValue("my-id")))
                    .Add(_ => _
                      .SetName("age")
                      .SetValue<IIntValueNode>(_ => _
                        .SetValueKind(ValueNodeKind.IntValue)
                        .SetValue(42)))))
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void FieldSelectionWithSelectionSet_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query {
        blogs {
          id
        }
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
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
                .SetArgumentList(null as IArgumentListNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(_ => _
                  .SetSelections(_ => _
                    .Add<FieldNodeBuilder>(_ => _
                      .SetSelectionKind(SelectionNodeKind.Field)
                      .SetAlias("")
                      .SetFieldName("id")
                      .SetArgumentList(null as IArgumentListNode)
                      .SetDirectiveList(null as IDirectiveListNode)
                      .SetSelectionSet(null as ISelectionSetNode))))))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void FragmentSpreadSelection_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query {
        ...blogs
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Query)
          .SetName("")
          .SetVariableDefinitionList(null as IVariableDefinitionListNode)
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FragmentSpreadNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.FragmentSpread)
                .SetFragmentName("blogs")
                .SetDirectiveList(null as IDirectiveListNode)))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void FragmentSpreadSelectionWithDirectives_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query {
        ...blogs @dir1
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Query)
          .SetName("")
          .SetVariableDefinitionList(null as IVariableDefinitionListNode)
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FragmentSpreadNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.FragmentSpread)
                .SetFragmentName("blogs")
                .SetDirectiveList(_ => _
                  .SetDirectives(_ => _
                    .Add(_ => _
                      .SetName("dir1")
                      .SetArgumentList(null as IArgumentListNode))))))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void InlineFragmentSelection_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query {
        ... on Type {
          name
          age
        }
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Query)
          .SetName("")
          .SetVariableDefinitionList(null as IVariableDefinitionListNode)
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<InlineFragmentNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.InlineFragment)
                .SetTypeCondition(_ => _
                  .SetType(_ => _
                    .SetName("Type")))
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(_ => _
                  .SetSelections(_ => _
                    .Add<FieldNodeBuilder>(_ => _
                      .SetSelectionKind(SelectionNodeKind.Field)
                      .SetAlias("")
                      .SetFieldName("name")
                      .SetArgumentList(null as IArgumentListNode)
                      .SetDirectiveList(null as IDirectiveListNode)
                      .SetSelectionSet(null as ISelectionSetNode))
                    .Add<FieldNodeBuilder>(_ => _
                      .SetSelectionKind(SelectionNodeKind.Field)
                      .SetAlias("")
                      .SetFieldName("age")
                      .SetArgumentList(null as IArgumentListNode)
                      .SetDirectiveList(null as IDirectiveListNode)
                      .SetSelectionSet(null as ISelectionSetNode))))))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void InlineFragmentSelectionWithDirectives_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query {
        ... on Type @dir1 {
          name
        }
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Query)
          .SetName("")
          .SetVariableDefinitionList(null as IVariableDefinitionListNode)
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<InlineFragmentNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.InlineFragment)
                .SetTypeCondition(_ => _
                  .SetType(_ => _
                    .SetName("Type")))
                .SetDirectiveList(_ => _
                  .SetDirectives(_ => _
                    .Add(_ => _
                      .SetName("dir1")
                      .SetArgumentList(null as IArgumentListNode))))
                .SetSelectionSet(_ => _
                  .SetSelections(_ => _
                    .Add<FieldNodeBuilder>(_ => _
                      .SetSelectionKind(SelectionNodeKind.Field)
                      .SetAlias("")
                      .SetFieldName("name")
                      .SetArgumentList(null as IArgumentListNode)
                      .SetDirectiveList(null as IDirectiveListNode)
                      .SetSelectionSet(null as ISelectionSetNode))))))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void InlineFragmentSelectionWithoutTypeCondition_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      query {
        ... {
          name
        }
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
      .SetDefinitions(_ => _
        .Add<OperationDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.OperationDefinition)
          .SetIsQueryShortHand(false)
          .SetOperationType(OperationType.Query)
          .SetName("")
          .SetVariableDefinitionList(null as IVariableDefinitionListNode)
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<InlineFragmentNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.InlineFragment)
                .SetTypeCondition(null as ITypeConditionNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(_ => _
                  .SetSelections(_ => _
                    .Add<FieldNodeBuilder>(_ => _
                      .SetSelectionKind(SelectionNodeKind.Field)
                      .SetAlias("")
                      .SetFieldName("name")
                      .SetArgumentList(null as IArgumentListNode)
                      .SetDirectiveList(null as IDirectiveListNode)
                      .SetSelectionSet(null as ISelectionSetNode))))))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Theory]
  [BlockStringData]
  public void BlockString_ShouldProduceCorrectAST(string blockString, IReadOnlyList<ReadOnlyMemory<char>> lines)
  {
    // Arrange
    string document = $$""""
      mutation {
        sendEmail(message: {{blockString}})
      }
      """";

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                        .SetLines(lines)))))
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryShortHandWithFragmentDefinition_ShouldProduceCorrectAST()
  {
    // Arrange
    string document = """
      {
        blogs
        ...myFragment @dir1
      }
      fragment myFragment on Query @dir2 {
        users
      }
      """;

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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

    // Act
    IDocumentNode actual = new RawNode(document);

    // Assert
    actual.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  private class BlockStringDataAttribute : DataAttribute
  {
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
      yield return ToData(Example1);
      yield return ToData(Example2);
      yield return ToData(Example3);
      yield return ToData(Example4);
      yield return ToData(Example5);
      yield return ToData(Example6);
    }

    private static (string, IReadOnlyList<string>) Example1
    {
      get
      {
        string blockString = """"
        """
            Hello,
              World!
        
            Yours,
              GraphQL.
          """
        """";

        IReadOnlyList<string> lines = new[]
        {
          "Hello,",
          "  World!",
          "",
          "Yours,",
          "  GraphQL."
        };

        return (blockString, lines);
      }
    }

    private static (string, IReadOnlyList<string>) Example2
    {
      get
      {
        string blockString = """"
        """Hello"""
        """";

        IReadOnlyList<string> lines = new[]
        {
          "Hello"
        };

        return (blockString, lines);
      }
    }

    private static (string, IReadOnlyList<string>) Example3
    {
      get
      {
        string blockString = """"
        """Hello,
        World!"""
        """";

        IReadOnlyList<string> lines = new[]
        {
          "Hello,",
          "World!"
        };

        return (blockString, lines);
      }
    }

    private static (string, IReadOnlyList<string>) Example4
    {
      get
      {
        string blockString = """"
        """Hello,
          World!"""
        """";

        IReadOnlyList<string> lines = new[]
        {
          "Hello,",
          "World!"
        };

        return (blockString, lines);
      }
    }

    private static (string, IReadOnlyList<string>) Example5
    {
      get
      {
        string blockString = """"
        """Hello,
          World!
          
          """
        """";

        IReadOnlyList<string> lines = new[]
        {
          "Hello,",
          "World!"
        };

        return (blockString, lines);
      }
    }

    private static (string, IReadOnlyList<string>) Example6
    {
      get
      {
        string blockString = """"
        """  Hello,
          World!"""
        """";

        IReadOnlyList<string> lines = new[]
        {
          "  Hello,",
          "World!"
        };

        return (blockString, lines);
      }
    }

    private static object[] ToData((string, IReadOnlyList<string>) pair) => new object[] { pair.Item1, pair.Item2.Select(line => line.AsMemory()).ToList() };
  }
}
