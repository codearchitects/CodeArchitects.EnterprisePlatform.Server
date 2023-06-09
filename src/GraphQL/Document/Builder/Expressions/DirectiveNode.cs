using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal class DirectiveNode : IteratorNode, IDirectiveNode,
  IEnumerable<IArgumentNode>, IEnumerator<IArgumentNode>
{
  private readonly INodeContext _context;

  public DirectiveNode(INodeContext context, string name, Expression expression)
  {
    _context = context;
    Name = name;
    Expression = expression;
  }

  protected override Expression Expression { get; }

  public string Name { get; }

  public IEnumerable<IArgumentNode> Arguments => this;

  IArgumentNode IEnumerator<IArgumentNode>.Current => GetCurrent<IArgumentNode>();

  IEnumerator<IArgumentNode> IEnumerable<IArgumentNode>.GetEnumerator() => GetEnumerator(MethodNames.WithArgument, this);

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodNames.WithArgument => NodeFactory.CreateArgument(_context, methodCall),
      _                        => throw new ExpressionEvaluationException(methodCall)
    };
  }
}
