using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class DirectiveNode : IteratorNode, IDirectiveNode,
  IArgumentListNode, IEnumerable<IArgumentNode>, IEnumerator<IArgumentNode>
{
  private readonly INodeRoot _root;
  private readonly string _name;

  public DirectiveNode(INodeRoot root, string name, Expression expression)
  {
    _root = root;
    _name = name;
    Expression = expression;
  }

  protected override Expression Expression { get; }

  public ReadOnlySpan<char> Name => _name;

  public IArgumentListNode? ArgumentList => Peek(MethodName.Represents.Argument) ? this : null;

  public IEnumerable<IArgumentNode> Arguments => this;

  IArgumentNode IEnumerator<IArgumentNode>.Current => GetCurrent<IArgumentNode>();

  IEnumerator<IArgumentNode> IEnumerable<IArgumentNode>.GetEnumerator() => GetEnumerator(MethodName.Represents.Argument, this);

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodName.WithArgument  => NodeFactory.CreateArgument(_root, methodCall),
      _                        => throw new ExpressionEvaluationException(methodCall)
    };
  }
}
