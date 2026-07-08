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
using Xunit.Sdk;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

public class NodesTests
{
  private const string s_queryName = "GetBlogs";
  private const string s_blogConnectionTypeName = "BlogConnection";

  private readonly Mock<INodeContext> _contextMock;

  public NodesTests()
  {
    _contextMock = new(MockBehavior.Strict);

    IObjectType? blogConnectionType = ObjectTypeBuilder.Build(_ => _
      .SetName(s_blogConnectionTypeName));

    LambdaExpression? defaultSelection = null;
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Connection<Blog>), out defaultSelection))
      .Returns(false);
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Edge<Blog>[]), out defaultSelection))
      .Returns(false);
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Blog[]), out defaultSelection))
      .Returns(false);
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(PageInfo), out defaultSelection))
      .Returns(false);
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(Guid), out defaultSelection))
      .Returns(false);
    _contextMock
      .Setup(x => x.TryGetDefaultSelection(typeof(string), out defaultSelection))
      .Returns(false);

    _contextMock
      .Setup(x => x.TryGetObjectType(typeof(Connection<Blog>), out blogConnectionType))
      .Returns(true);
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, variables, expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Theory]
  [LiteralValueData]
  public void QueryBlogFieldWithLiteralArgument_ShouldProduceCorrectAST(IValueNode value, object? literalValue)
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
          .WithArgument("literalArg", literalValue))
      });

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogFieldWithVariableArgument_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot, GetBlogsVariables>, IBuildResult<GetBlogsResult, GetBlogsVariables>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                        .SetName(nameof(GetBlogsVariables.Arg1))))
                    .Add(_ => _
                      .SetName(nameof(GetBlogsVariables.Arg2))
                      .SetValue<IVariableNode>(_ => _
                        .SetValueKind(ValueNodeKind.Variable)
                        .SetName(nameof(GetBlogsVariables.Arg2))))))
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogFieldWithInlineObjectArgument_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                      .SetName("inlineObj")
                      .SetValue(ObjectValueNodeBuilder.Build(_ => _
                        .SetFields(_ => _
                          .Add(_ => _
                            .SetName("ScalarField")
                            .SetValue<IIntValueNode>(_ => _
                              .SetValueKind(ValueNodeKind.IntValue)
                              .SetValue(1)))
                          .Add(_ => _
                            .SetName("ObjectField")
                            .SetValue(ObjectValueNodeBuilder.Build(_ => _
                              .SetFields(_ => _
                                .Add(_ => _
                                  .SetName("InnerField1")
                                  .SetValue<IStringValueNode>(_ => _
                                    .SetValueKind(ValueNodeKind.StringValue)
                                    .SetValue("inner-field-1")))
                                .Add(_ => _
                                  .SetName("InnerField2")
                                  .SetValue<IIntValueNode>(_ => _
                                    .SetValueKind(ValueNodeKind.IntValue)
                                    .SetValue(2)))))))
                          .Add(_ => _
                            .SetName("ListField")
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
                                  .SetValue(3)))))))))))
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
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
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
          .WithArgument("runtimeObj", obj))
      });

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                      .SetName("runtimeObj")
                      .SetValue<IObjectValueNode>(_ => _
                        .SetFields(_ => _
                          .Add(_ => _
                            .SetName("ScalarField")
                            .SetValue<IIntValueNode>(_ => _
                              .SetValueKind(ValueNodeKind.IntValue)
                              .SetValue(1)))
                          .Add(_ => _
                            .SetName("ObjectField")
                            .SetValue<IObjectValueNode>(_ => _
                              .SetFields(_ => _
                                .Add(_ => _
                                  .SetName("InnerField1")
                                  .SetValue<IStringValueNode>(_ => _
                                    .SetValueKind(ValueNodeKind.StringValue)
                                    .SetValue("inner-field-1")))
                                .Add(_ => _
                                  .SetName("InnerField2")
                                  .SetValue<IIntValueNode>(_ => _
                                    .SetValueKind(ValueNodeKind.IntValue)
                                    .SetValue(2))))))
                          .Add(_ => _
                            .SetName("ListField")
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

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogFieldWithFieldDirectives_ShouldProduceCorrectAST()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
          .WithDirective("directive1")
          .WithDirective("directive2", _ => _
            .WithArgument("directiveArg", 1)))
      });

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                .SetArgumentList(null as IArgumentListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                .SetAlias("Blogs")
                .SetFieldName("connectionBlogs")
                .SetDirectiveList(null as IDirectiveListNode)
                .SetArgumentList(null as IArgumentListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                .SetAlias("Blogs")
                .SetFieldName("blogsConnection")
                .SetDirectiveList(null as IDirectiveListNode)
                .SetArgumentList(null as IArgumentListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                      .SetSelectionSet(null as ISelectionSetNode))))))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenSelectionHasExpandCallToNamedType()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
          .WithSelection(connection => new Connection<Blog>
          {
            Edges = connection.Edges.Expand(_, _ => _
              .WithSelection(edge => new Edge<Blog>
              {
                Cursor = edge.Cursor
              }))
          }))
      });

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                            .SetSelectionSet(null as ISelectionSetNode)))))))))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenSelectionHasExpandCallToAnonymousType()
  {
    // Arrange
    var expansion = Infer(_ => _
      .WithSelection(root => new
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
          .WithSelection(connection => new
          {
            Edges = connection.Edges.Expand(_, _ => _
              .WithSelection(edge => new
              {
                edge.Cursor
              }))
          }))
      }));

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                            .SetSelectionSet(null as ISelectionSetNode)))))))))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));

    static Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<T>>> Infer<T>(Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<T>>> expansion) // Helper function to allow the compiler to infer the anonymous type
      => expansion;
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenDefaultSelectionHasExpandCall()
  {
    // Arrange
    Expression<Func<Connection<Blog>, Connection<Blog>>> defaultConnectionSelectionLambda = connection => new Connection<Blog>
    {
      Edges = connection.Edges.Expand(default(IBuilder)!, _ => _
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                            .SetSelectionSet(null as ISelectionSetNode)))))))))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenSelectionHasSelectCall()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Select(_, connection => new Connection<Blog>
        {
          Edges = connection.Edges.Select(_, edge => new Edge<Blog>
          {
            Cursor = edge.Cursor
          })
        })
      });

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                            .SetSelectionSet(null as ISelectionSetNode)))))))))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenDefaultSelectionHasSelectCall()
  {
    // Arrange
    Expression<Func<Connection<Blog>, Connection<Blog>>> defaultConnectionSelectionLambda = connection => new Connection<Blog>
    {
      Edges = connection.Edges.Select(default(IBuilder)!, edge => new Edge<Blog>
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

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                            .SetSelectionSet(null as ISelectionSetNode)))))))))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenUsingInlineFragmentOfDerivedType()
  {
    // Arrange
    string scienceBlogName = nameof(ScienceBlog);

    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
          .WithSelection(connection => new Connection<Blog>
          {
            Nodes = connection.Nodes.Expand(_, _ => _
              .WithSelectionSet(_ => _
                .WithSelection(blog => new Blog
                {
                  Id = blog.Id,
                  Name = blog.Name
                })
                .WithInlineFragment<ScienceBlog>(_ => _
                  .WithDirective("dir1")
                  .WithSelection(blog => new
                  {
                    blog.Theme
                  }))))
          }))
      });

    IObjectType? fragmentType = ObjectTypeBuilder.Build(_ => _
      .SetName(scienceBlogName));

    _contextMock
      .Setup(x => x.TryGetObjectType(typeof(ScienceBlog), out fragmentType))
      .Returns(true);

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                      .SetFieldName("Nodes")
                      .SetDirectiveList(null as IDirectiveListNode)
                      .SetArgumentList(null as IArgumentListNode)
                      .SetSelectionSet(_ => _
                        .SetSelections(_ => _
                          .Add<FieldNodeBuilder>(_ => _
                            .SetSelectionKind(SelectionNodeKind.Field)
                            .SetAlias("")
                            .SetFieldName("Id")
                            .SetDirectiveList(null as IDirectiveListNode)
                            .SetArgumentList(null as IArgumentListNode)
                            .SetSelectionSet(null as ISelectionSetNode))
                          .Add<FieldNodeBuilder>(_ => _
                            .SetSelectionKind(SelectionNodeKind.Field)
                            .SetAlias("")
                            .SetFieldName("Name")
                            .SetDirectiveList(null as IDirectiveListNode)
                            .SetArgumentList(null as IArgumentListNode)
                            .SetSelectionSet(null as ISelectionSetNode))
                          .Add<InlineFragmentNodeBuilder>(_ => _
                            .SetSelectionKind(SelectionNodeKind.InlineFragment)
                            .SetDirectiveList(_ => _
                              .SetDirectives(_ => _
                                .Add(_ => _
                                  .SetName("dir1")
                                  .SetArgumentList(null as IArgumentListNode))))
                            .SetTypeCondition(_ => _
                              .SetType(_ => _
                                .SetName(scienceBlogName)))
                            .SetSelectionSet(_ => _
                              .SetSelections(_ => _
                                .Add<FieldNodeBuilder>(_ => _
                                  .SetSelectionKind(SelectionNodeKind.Field)
                                  .SetAlias("")
                                  .SetFieldName("Theme")
                                  .SetDirectiveList(null as IDirectiveListNode)
                                  .SetArgumentList(null as IArgumentListNode)
                                  .SetSelectionSet(null as ISelectionSetNode))))))))))))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenUsingInlineFragmentOfSameType()
  {
    // Arrange
    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
          .WithSelection(connection => new Connection<Blog>
          {
            Nodes = connection.Nodes.Expand(_, _ => _
              .WithSelectionSet(_ => _
                .WithSelection(blog => new Blog
                {
                  Id = blog.Id
                })
                .WithInlineFragment(_ => _
                  .WithSelection(blog => new
                  {
                    blog.Name
                  }))))
          }))
      });

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                      .SetFieldName("Nodes")
                      .SetDirectiveList(null as IDirectiveListNode)
                      .SetArgumentList(null as IArgumentListNode)
                      .SetSelectionSet(_ => _
                        .SetSelections(_ => _
                          .Add<FieldNodeBuilder>(_ => _
                            .SetSelectionKind(SelectionNodeKind.Field)
                            .SetAlias("")
                            .SetFieldName("Id")
                            .SetDirectiveList(null as IDirectiveListNode)
                            .SetArgumentList(null as IArgumentListNode)
                            .SetSelectionSet(null as ISelectionSetNode))
                          .Add<InlineFragmentNodeBuilder>(_ => _
                            .SetSelectionKind(SelectionNodeKind.InlineFragment)
                            .SetTypeCondition(null as ITypeConditionNode)
                            .SetDirectiveList(null as IDirectiveListNode)
                            .SetSelectionSet(_ => _
                              .SetSelections(_ => _
                                .Add<FieldNodeBuilder>(_ => _
                                  .SetSelectionKind(SelectionNodeKind.Field)
                                  .SetAlias("")
                                  .SetFieldName("Name")
                                  .SetDirectiveList(null as IDirectiveListNode)
                                  .SetArgumentList(null as IArgumentListNode)
                                  .SetSelectionSet(null as ISelectionSetNode))))))))))))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  [Fact]
  public void QueryBlogField_ShouldProduceCorrectAST_WhenSelectionHasFragments()
  {
    // Arrange
    string fragment1Name = "fragment1";
    string fragment2Name = "fragment2";

    Expression<Func<IFragmentBuilder<Connection<Blog>>, IBuildResult<Connection<Blog>>>> fragment1Expression = _ => _
      .WithSelection(connection => new Connection<Blog>
      {
        PageInfo = connection.PageInfo
      });
    Expression<Func<IFragmentBuilder<Connection<Blog>>, IBuildResult<Connection<Blog>>>> fragment2Expression = _ => _
      .WithDirective("dir1", _ => _
        .WithArgument("arg1", 1))
      .WithSelection(connection => new Connection<Blog>
      {
        Edges = connection.Edges
      });

    GraphFragment<Connection<Blog>> fragment1 = new(fragment1Name, fragment1Expression.Body);
    GraphFragment<Connection<Blog>> fragment2 = new(fragment2Name, fragment2Expression.Body);

    Expression<Func<IOperationBuilder<IDocumentRoot>, IBuildResult<GetBlogsResult>>> expansion = _ => _
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = root.Field<Connection<Blog>>("blogs").Expand(_, _ => _
          .WithSelectionSet(_ => _
            .WithSelection(connection => new Connection<Blog>
            {
              Nodes = connection.Nodes
            })
            .WithFragmentSpread(fragment1)
            .WithFragmentSpread(fragment2, _ => _
              .WithDirective("dir2", _ => _
                .WithArgument("arg", "arg-value")))))
      });

    IDocumentNode expected = DocumentNodeBuilder.Build(_ => _
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
                      .SetFieldName("Nodes")
                      .SetDirectiveList(null as IDirectiveListNode)
                      .SetArgumentList(null as IArgumentListNode)
                      .SetSelectionSet(null as ISelectionSetNode))
                    .Add<FragmentSpreadNodeBuilder>(_ => _
                      .SetSelectionKind(SelectionNodeKind.FragmentSpread)
                      .SetFragmentName(fragment1Name)
                      .SetDirectiveList(null as IDirectiveListNode))
                    .Add<FragmentSpreadNodeBuilder>(_ => _
                      .SetSelectionKind(SelectionNodeKind.FragmentSpread)
                      .SetFragmentName(fragment2Name)
                      .SetDirectiveList(_ => _
                        .SetDirectives(_ => _
                          .Add(_ => _
                            .SetName("dir2")
                            .SetArgumentList(_ => _
                              .SetArguments(_ => _
                                .Add(_ => _
                                  .SetName("arg")
                                  .SetValue<IStringValueNode>(_ => _
                                    .SetValueKind(ValueNodeKind.StringValue)
                                    .SetValue("arg-value")))))))))))))))
        .Add<FragmentDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.FragmentDefinition)
          .SetFragmentName(fragment1Name)
          .SetTypeCondition(_ => _
            .SetType(_ => _
              .SetName(s_blogConnectionTypeName)))
          .SetDirectiveList(null as IDirectiveListNode)
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FieldNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.Field)
                .SetAlias("")
                .SetFieldName("PageInfo")
                .SetArgumentList(null as IArgumentListNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))
        .Add<FragmentDefinitionNodeBuilder>(_ => _
          .SetDefinitionKind(DefinitionNodeKind.FragmentDefinition)
          .SetFragmentName(fragment2Name)
          .SetTypeCondition(_ => _
            .SetType(_ => _
              .SetName(s_blogConnectionTypeName)))
          .SetDirectiveList(_ => _
            .SetDirectives(_ => _
              .Add(_ => _
                .SetName("dir1")
                .SetArgumentList(_ => _
                  .SetArguments(_ => _
                    .Add(_ => _
                      .SetName("arg1")
                      .SetValue<IIntValueNode>(_ => _
                        .SetValueKind(ValueNodeKind.IntValue)
                        .SetValue(1))))))))
          .SetSelectionSet(_ => _
            .SetSelections(_ => _
              .Add<FieldNodeBuilder>(_ => _
                .SetSelectionKind(SelectionNodeKind.Field)
                .SetAlias("")
                .SetFieldName("Edges")
                .SetArgumentList(null as IArgumentListNode)
                .SetDirectiveList(null as IDirectiveListNode)
                .SetSelectionSet(null as ISelectionSetNode)))))));

    // Act
    IDocumentNode node = new QueryDocumentNode(_contextMock.Object, s_queryName, Array.Empty<IVariable>(), expansion.Body);

    // Assert
    node.Should().BeEquivalentTo(expected, opt => opt.Using(NodeEqualityComparer.Instance as IEqualityComparer<IDocumentNode>));
  }

  private sealed class LiteralValueDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
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

      yield return new object?[] { intValue, 1 };
      yield return new object?[] { floatValue, 1.5 };
      yield return new object?[] { stringValue, "literal-string" };
      yield return new object?[] { trueValue, true };
      yield return new object?[] { falseValue, false };
      yield return new object?[] { nullValue, null };
    }
  }
}
