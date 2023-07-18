using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Nodes.FluentMock;
using CodeArchitects.Platform.GraphQL.Fixtures;
using FluentAssertions;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetValue("my-id")))))))
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("")
            .SetFieldName("blogs")
            .SetArgumentList(null as IArgumentListNode)
            .SetDirectiveList(null as IDirectiveListNode)
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetValue("my-id"))
                .Add(_ => _
                  .SetName("age")
                  .SetValue(42))))
            .SetDirectiveList(null as IDirectiveListNode)
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetSelectionSet(null as ISelectionSetNode))))))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName("")
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FragmentSpreadNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.FragmentSpread)
            .SetFragmentName("blogs")
            .SetDirectiveList(null as IDirectiveListNode)))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetArgumentList(null as IArgumentListNode))))))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetSelectionSet(null as ISelectionSetNode))))))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetSelectionSet(null as ISelectionSetNode))))))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetSelectionSet(null as ISelectionSetNode))))))));

    // Act
    LiteralNode queryDefinition = new(document);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }
}
