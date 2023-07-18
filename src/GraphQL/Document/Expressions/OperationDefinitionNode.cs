using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Builder;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal abstract class OperationDefinitionNode : IteratorNode, IOperationDefinitionNode,
  IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  private readonly INodeContext _context;
  private readonly string _name;
  private readonly IReadOnlyList<IVariable> _variables;

  public OperationDefinitionNode(INodeContext context, string name, IReadOnlyList<IVariable> variables, Expression expression)
  {
    _context = context;
    _name = name;
    _variables = variables;
    Expression = expression;
  }

  protected sealed override Expression Expression { get; }

  public abstract OperationType OperationType { get; }

  public ReadOnlySpan<char> Name => _name;

  public IVariableDefinitionListNode? VariableDefinitionList => _variables.Count == 0
    ? null
    : new VariableDefinitionListNode(_variables);

  public IDirectiveListNode? DirectiveList => Peek(MethodNames.WithDirective) ? this : null;

  public IEnumerable<IDirectiveNode> Directives => this;

  public ISelectionSetNode SelectionSet => GetNext<ISelectionSetNode>(MethodNames.WithSelection);

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => GetCurrent<IDirectiveNode>();

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator() => GetEnumerator(MethodNames.WithDirective, this);

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodNames.WithDirective => NodeFactory.CreateDirective(_context, methodCall),
      MethodNames.WithSelection => NodeFactory.CreateSelectionSet(_context, methodCall),
      _                         => throw new ExpressionEvaluationException(methodCall),
    };
  }
}
