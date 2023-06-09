using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Builder;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal abstract class OperationDefinitionNode : IteratorNode, IOperationDefinitionNode,
  IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  private readonly INodeContext _context;

  public OperationDefinitionNode(INodeContext context, string? name, IEnumerable<IVariable> variables, Expression expression)
  {
    _context = context;
    Name = name;
    Variables = variables;
    Expression = expression;
  }

  protected sealed override Expression Expression { get; }

  public abstract OperationType OperationType { get; }

  public string? Name { get; }

  public IEnumerable<IVariable> Variables { get; }

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
