using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal abstract class OperationDefinitionNode : IteratorNode, IOperationDefinitionNode,
  IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  private readonly INodeRoot _root;
  private readonly string _name;
  private readonly IReadOnlyList<IVariable> _variables;

  public OperationDefinitionNode(INodeRoot root, string name, IReadOnlyList<IVariable> variables, Expression expression)
  {
    _root = root;
    _name = name;
    _variables = variables;
    Expression = expression;
  }

  protected sealed override Expression Expression { get; }

  public DefinitionNodeKind DefinitionKind => DefinitionNodeKind.OperationDefinition;

  public bool IsQueryShortHand => false;

  public abstract OperationType OperationType { get; }

  public ReadOnlySpan<char> Name => _name;

  public IVariableDefinitionListNode? VariableDefinitionList => _variables.Count == 0
    ? null
    : new VariableDefinitionListNode(_variables);

  public IDirectiveListNode? DirectiveList => Peek(MethodNames.IsDirective) ? this : null;

  public IEnumerable<IDirectiveNode> Directives => this;

  public ISelectionSetNode SelectionSet => GetNext<ISelectionSetNode>(MethodNames.IsSelectionSet);

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => GetCurrent<IDirectiveNode>();

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator() => GetEnumerator(MethodNames.IsDirective, this);

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodNames.WithDirective    => NodeFactory.CreateDirective(_root, methodCall),
      MethodNames.WithSelection    => NodeFactory.CreateSimpleSelectionSet(_root, methodCall),
      MethodNames.WithSelectionSet => NodeFactory.CreateSelectionSet(_root, methodCall),
      _                            => throw new ExpressionEvaluationException(methodCall)
    };
  }
}
