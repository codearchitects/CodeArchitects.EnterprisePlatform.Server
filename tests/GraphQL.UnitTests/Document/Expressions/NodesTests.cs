using CodeArchitects.Platform.GraphQL.Document.Builder;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Nodes.FluentMock;
using CodeArchitects.Platform.GraphQL.Fixtures;
using CodeArchitects.Platform.GraphQL.Fixtures.Model;
using CodeArchitects.Platform.GraphQL.Model;
using CodeArchitects.Platform.GraphQL.Model.FluentMock;
using CodeArchitects.Platform.GraphQL.UnitTests.FluentMock;
using FluentAssertions;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

public class NodesTests
{
  private const string s_queryName = "GetBlogs";

  private readonly Mock<INodeContext> _contextMock;

  public NodesTests()
  {
    _contextMock = new(MockBehavior.Strict);

    LambdaExpression? defaultSelection = null;
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Connection<Blog>), out defaultSelection))
      .Returns(false);
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Edge<Blog>[]), out defaultSelection))
      .Returns(false);
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(PageInfo), out defaultSelection))
      .Returns(false);
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(string), out defaultSelection))
      .Returns(false);
  }

  [Fact]
  public void QueryBlogFieldWithoutVariables_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs")
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogFieldWithVariables_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot, GetBlogsVariables>, IBuildResult<GetBlogsResult, GetBlogsVariables>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs")
      });

    IReadOnlyList<IVariable> variables = ListBuilder<IVariable, VariableBuilder>.Build(_ => _
      .Add(_ => _
        .SetName("arg1")
        .SetType<IScalarType>(_ => _
          .SetKind(TypeKind.Scalar)
          .SetName("Integer")
          .SetIsNullable(false)))
      .Add(_ => _
        .SetName("arg2")
        .SetType<IScalarType>(_ => _
          .SetKind(TypeKind.Scalar)
          .SetName("String")
          .SetIsNullable(true)))
      .Add(_ => _
        .SetName("arg3")
        .SetType<IListType>(_ => _
          .SetKind(TypeKind.List)
          .SetIsNullable(false)
          .SetItemType<IScalarType>(_ => _
            .SetKind(TypeKind.Scalar)
            .SetName("Integer")
            .SetIsNullable(false))))
      .Add(_ => _
        .SetName("arg4")
        .SetType<IListType>(_ => _
          .SetKind(TypeKind.List)
          .SetIsNullable(true)
          .SetItemType<IScalarType>(_ => _
            .SetKind(TypeKind.Scalar)
            .SetName("Integer")
            .SetIsNullable(true)))));
  
    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariableDefinitionList(_ => _
        .SetVariableDefinitions(_ => _
          .Add(_ => _
            .SetVariable(_ => _
              .SetName("arg1"))
            .SetType<INonNullTypeNode>(_ => _
              .SetTypeKind(TypeNodeKind.NonNullType)
              .SetNullableType<INamedTypeNode>(_ => _
                .SetTypeKind(TypeNodeKind.NamedType)
                .SetName("Integer"))))
          .Add(_ => _
            .SetVariable(_ => _
              .SetName("arg2"))
            .SetType<INamedTypeNode>(_ => _
              .SetTypeKind(TypeNodeKind.NamedType)
              .SetName("String")))
          .Add(_ => _
            .SetVariable(_ => _
              .SetName("arg3"))
            .SetType<INonNullTypeNode>(_ => _
              .SetTypeKind(TypeNodeKind.NonNullType)
              .SetNullableType<IListTypeNode>(_ => _
                .SetTypeKind(TypeNodeKind.ListType)
                .SetItemType<INonNullTypeNode>(_ => _
                  .SetTypeKind(TypeNodeKind.NonNullType)
                  .SetNullableType<INamedTypeNode>(_ => _
                    .SetTypeKind(TypeNodeKind.NamedType)
                    .SetName("Integer"))))))
          .Add(_ => _
            .SetVariable(_ => _
              .SetName("arg4"))
            .SetType<IListTypeNode>(_ => _
              .SetTypeKind(TypeNodeKind.ListType)
              .SetItemType<INamedTypeNode>(_ => _
                .SetTypeKind(TypeNodeKind.NamedType)
                .SetName("Integer"))))))
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
  
    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, variables, expansion.Body);
  
    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Theory]
  [InlineData(1)]
  [InlineData("literal-string")]
  [InlineData(false)]
  [InlineData(null)]
  public void QueryBlogFieldWithLiteralArgument_ShouldProduceCorrectAST(object? value)
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.ExpandRef(root.Field<Connection<Blog>>("blogs"), _ => _
          .WithArgument("literalArg", value))
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogFieldWithVariableArgument_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot, GetBlogsVariables>, IBuildResult<GetBlogsResult, GetBlogsVariables>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.ExpandRef(root.Field<Connection<Blog>>("blogs"), _ => _
          .WithArgument("first", vars => vars.Arg1)
          .WithArgument(vars => vars.Arg2))
      });

    _contextMock
      .Setup(x => x.GetVariable(typeof(GetBlogsVariables).GetRequiredProperty(nameof(GetBlogsVariables.Arg1))))
      .Returns(VariableBuilder.Build(_ => _
        .SetName(nameof(GetBlogsVariables.Arg1))));

    _contextMock
      .Setup(x => x.GetVariable(typeof(GetBlogsVariables).GetRequiredProperty(nameof(GetBlogsVariables.Arg2))))
      .Returns(VariableBuilder.Build(_ => _
        .SetName(nameof(GetBlogsVariables.Arg2))));

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                    .SetName(nameof(GetBlogsVariables.Arg1)))))
                .Add(_ => _
                  .SetName(nameof(GetBlogsVariables.Arg2))
                  .SetValue(VariableNodeBuilder.Build(_ => _
                    .SetName(nameof(GetBlogsVariables.Arg2)))))))
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogFieldWithInlineObjectArgument_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.ExpandRef(root.Field<Connection<Blog>>("blogs"), _ => _
          .WithArgument("inlineObj", new
          {
            ScalarField = 1,
            ObjectField = new
            {
              InnerField1 = "inner-field-1",
              InnerField2 = 2
            },
            ListField = new[] { 1, 2, 3 }
          }))
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetName("inlineObj")
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
                          .SetValues(new object?[] { 1, 2, 3 }))))))))))
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogFieldWithRuntimeObjectArgument_ShouldProduceCorrectAST()
  {
    // Arrange
    var obj = new
    {
      ScalarField = 1,
      ObjectField = new
      {
        InnerField1 = "inner-field-1",
        InnerField2 = 2
      },
      ListField = new[] { 1, 2, 3 }
    };

    IObjectType? objType = ObjectTypeBuilder.Build(_ => _
      .SetFields(_ => _
        .Add(_ => _
          .SetName("ScalarField")
          .SetGetValue(obj => ((dynamic)obj).ScalarField))
        .Add(_ => _
          .SetName("ObjectField")
          .SetGetValue(obj => ((dynamic)obj).ObjectField))
        .Add(_ => _
          .SetName("ListField")
          .SetGetValue(obj => ((dynamic)obj).ListField))));
    _contextMock
      .Setup(x => x.TryGetObjectType(obj.GetType(), out objType))
      .Returns(true);

    IObjectType? objFieldType = ObjectTypeBuilder.Build(_ => _
      .SetFields(_ => _
        .Add(_ => _
          .SetName("InnerField1")
          .SetGetValue(obj => ((dynamic)obj).InnerField1))
        .Add(_ => _
          .SetName("InnerField2")
          .SetGetValue(obj => ((dynamic)obj).InnerField2))));
    _contextMock
      .Setup(x => x.TryGetObjectType(obj.ObjectField.GetType(), out objFieldType))
      .Returns(true);

    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.ExpandRef(root.Field<Connection<Blog>>("blogs"), _ => _
          .WithArgument("runtimeObj", obj))
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetName("runtimeObj")
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
                          .SetValues(new object?[] { 1, 2, 3 }))))))))))
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogFieldWithOperationDirectives_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithDirective("directive1")
      .WithDirective("directive2", _ => _
        .WithArgument("directiveArg", 1))
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs")
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogFieldWithFieldDirectives_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.ExpandRef(root.Field<Connection<Blog>>("blogs"), _ => _
          .WithDirective("directive1")
          .WithDirective("directive2", _ => _
            .WithArgument("directiveArg", 1)))
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
            .SetArgumentList(null as IArgumentListNode)
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogFieldWithAlias_ShouldProduceCorrectAST_WhenNamesDifferInTheFirstLetter()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("connectionBlogs")
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("Blogs")
            .SetFieldName("connectionBlogs")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(null as IArgumentListNode)
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogFieldWithAlias_ShouldProduceCorrectAST_WhenNamesDifferNotInTheFirstLetter()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogsConnection")
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
      .SetOperationType(OperationType.Query)
      .SetName(s_queryName)
      .SetVariableDefinitionList(null as IVariableDefinitionListNode)
      .SetDirectiveList(null as IDirectiveListNode)
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add<FieldNodeBuilder>(_ => _
            .SetSelectionKind(SelectionNodeKind.Field)
            .SetAlias("Blogs")
            .SetFieldName("blogsConnection")
            .SetDirectiveList(null as IDirectiveListNode)
            .SetArgumentList(null as IArgumentListNode)
            .SetSelectionSet(null as ISelectionSetNode)))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenDefaultSelectionHasMemberAssignmentsOnly()
  {
    // Arrange
    Expression<Func<Connection<Blog>, Connection<Blog>>> defaultConnectionSelectionLambda = connection => new Connection<Blog>
    {
      Edges = connection.Edges,
      PageInfo = connection.PageInfo
    };
    LambdaExpression? defaultConnectionSelection = defaultConnectionSelectionLambda;

    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Connection<Blog>), out defaultConnectionSelection))
      .Returns(true);

    Expression<Func<Edge<Blog>, Edge<Blog>>> defaultEdgeSelectionLambda = edge => new Edge<Blog>
    {
      Cursor = edge.Cursor
    };
    LambdaExpression? defaultEdgeSelection = defaultEdgeSelectionLambda;

    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Edge<Blog>[]), out defaultEdgeSelection))
      .Returns(true);

    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs")
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetFieldName("Edges")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add<FieldNodeBuilder>(_ => _
                        .SetSelectionKind(SelectionNodeKind.Field)
                        .SetAlias("")
                        .SetFieldName("Cursor")
                        .SetDirectiveList(null as IDirectiveListNode)
                        .SetArgumentList(null as IArgumentListNode)
                        .SetSelectionSet(null as ISelectionSetNode)))))
                .Add<FieldNodeBuilder>(_ => _
                  .SetSelectionKind(SelectionNodeKind.Field)
                  .SetAlias("")
                  .SetFieldName("PageInfo")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(null as ISelectionSetNode))))))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenSelectionHasExpandCallToNamedType()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.ExpandRef(root.Field<Connection<Blog>>("blogs"), _ => _
          .WithSelection(connection => new Connection<Blog>
          {
            Edges = _.ExpandCol(connection.Edges, _ => _
              .WithSelection(edge => new Edge<Blog>
              {
                Cursor = edge.Cursor
              }))
          }))
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetFieldName("Edges")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add<FieldNodeBuilder>(_ => _
                        .SetSelectionKind(SelectionNodeKind.Field)
                        .SetAlias("")
                        .SetFieldName("Cursor")
                        .SetDirectiveList(null as IDirectiveListNode)
                        .SetArgumentList(null as IArgumentListNode)
                        .SetSelectionSet(null as ISelectionSetNode)))))))))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenSelectionHasExpandCallToAnonymousType()
  {
    // Arrange
    var expansion = Infer(_ => _
      .WithSelection(root => new
      {
        Blogs = _.ExpandRef(root.Field<Connection<Blog>>("blogs"), _ => _
          .WithSelection(connection => new
          {
            Edges = _.ExpandCol(connection.Edges, _ => _
              .WithSelection(edge => new
              {
                edge.Cursor
              }))
          }))
      }));

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetFieldName("Edges")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add<FieldNodeBuilder>(_ => _
                        .SetSelectionKind(SelectionNodeKind.Field)
                        .SetAlias("")
                        .SetFieldName("Cursor")
                        .SetDirectiveList(null as IDirectiveListNode)
                        .SetArgumentList(null as IArgumentListNode)
                        .SetSelectionSet(null as ISelectionSetNode)))))))))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));

    static Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<T>>> Infer<T>(Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<T>>> expansion) // Helper function to allow the compiler to infer the anonymous type
      => expansion;
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenDefaultSelectionHasExpandCall()
  {
    // Arrange
    Expression<Func<Connection<Blog>, Connection<Blog>>> defaultConnectionSelectionLambda = connection => new Connection<Blog>
    {
      Edges = GQL.ExpandCol(connection.Edges, _ => _
        .WithSelection(edge => new Edge<Blog>
        {
          Cursor = edge.Cursor
        }))
    };
    LambdaExpression? defaultConnectionSelection = defaultConnectionSelectionLambda;

    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Connection<Blog>), out defaultConnectionSelection))
      .Returns(true);

    LambdaExpression? defaultEdgeSelection = null;

    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Edge<Blog>[]), out defaultEdgeSelection))
      .Throws(new InvalidOperationException("Should not have gotten here"));

    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs")
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetFieldName("Edges")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add<FieldNodeBuilder>(_ => _
                        .SetSelectionKind(SelectionNodeKind.Field)
                        .SetAlias("")
                        .SetFieldName("Cursor")
                        .SetDirectiveList(null as IDirectiveListNode)
                        .SetArgumentList(null as IArgumentListNode)
                        .SetSelectionSet(null as ISelectionSetNode)))))))))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenSelectionHasSelectCall()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.SelectRef(root.Field<Connection<Blog>>("blogs"), connection => new Connection<Blog>
        {
          Edges = _.SelectCol(connection.Edges, edge => new Edge<Blog>
          {
            Cursor = edge.Cursor
          })
        })
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetFieldName("Edges")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add<FieldNodeBuilder>(_ => _
                        .SetSelectionKind(SelectionNodeKind.Field)
                        .SetAlias("")
                        .SetFieldName("Cursor")
                        .SetDirectiveList(null as IDirectiveListNode)
                        .SetArgumentList(null as IArgumentListNode)
                        .SetSelectionSet(null as ISelectionSetNode)))))))))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenDefaultSelectionHasSelectCall()
  {
    // Arrange
    Expression<Func<Connection<Blog>, Connection<Blog>>> defaultConnectionSelectionLambda = connection => new Connection<Blog>
    {
      Edges = GQL.SelectCol(connection.Edges, edge => new Edge<Blog>
      {
        Cursor = edge.Cursor
      })
    };
    LambdaExpression? defaultConnectionSelection = defaultConnectionSelectionLambda;

    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Connection<Blog>), out defaultConnectionSelection))
      .Returns(true);

    LambdaExpression? defaultEdgeSelection = null;

    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Edge<Blog>[]), out defaultEdgeSelection))
      .Throws(new InvalidOperationException("Should not have gotten here"));

    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs")
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                  .SetFieldName("Edges")
                  .SetDirectiveList(null as IDirectiveListNode)
                  .SetArgumentList(null as IArgumentListNode)
                  .SetSelectionSet(_ => _
                    .SetSelections(_ => _
                      .Add<FieldNodeBuilder>(_ => _
                        .SetSelectionKind(SelectionNodeKind.Field)
                        .SetAlias("")
                        .SetFieldName("Cursor")
                        .SetDirectiveList(null as IDirectiveListNode)
                        .SetArgumentList(null as IArgumentListNode)
                        .SetSelectionSet(null as ISelectionSetNode)))))))))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }
}
