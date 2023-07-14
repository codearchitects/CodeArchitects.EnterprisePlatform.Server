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

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
    Expression<Func<IOperationBuilder<IDocumentRoot, GetBlogsVariables>, IBuildResult<GetBlogsResult, GetBlogsVariables>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.ExpandRef(root.Field<Connection<Blog>>("blogs"), _ => _
          .WithArgument("literalArg", value))
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
          .WithArgument("first", vars => vars.First)
          .WithArgument(vars => vars.Last))
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }

  [Fact]
  public void QueryBlogFieldWithInlineObjectArgument_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot, GetBlogsVariables>, IBuildResult<GetBlogsResult, GetBlogsVariables>>> expansion = _ => _
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
                        .SetValues(new object?[] { 1, 2, 3 })))))))))
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

    Expression<Func<IOperationBuilder<IDocumentRoot, GetBlogsVariables>, IBuildResult<GetBlogsResult, GetBlogsVariables>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.ExpandRef(root.Field<Connection<Blog>>("blogs"), _ => _
          .WithArgument("runtimeObj", obj))
      });

    IOperationDefinitionNode expected = OperationDefinitionNodeBuilder.Build(_ => _
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
                        .SetValues(new object?[] { 1, 2, 3 })))))))))
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
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias(null)
            .SetFieldName("blogs")
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
            .SetArguments()
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
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias("Blogs")
            .SetFieldName("connectionBlogs")
            .SetDirectives()
            .SetArguments()
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
      .SetVariables()
      .SetDirectives()
      .SetSelectionSet(_ => _
        .SetSelections(_ => _
          .Add(_ => _
            .SetAlias("Blogs")
            .SetFieldName("blogsConnection")
            .SetDirectives()
            .SetArguments()
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
                        .SetSelectionSet(null as ISelectionSetNode)))))))))));

    // Act
    IOperationDefinitionNode queryDefinition = new QueryDefinitionNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    queryDefinition.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IOperationDefinitionNode>));
  }
}
