using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class FragmentSpreadNode : IteratorNode, IFragmentSpreadNode,
  IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  private readonly INodeRoot _root;
  private readonly GraphFragment _fragment;

  public FragmentSpreadNode(INodeRoot root, GraphFragment fragment, Expression expression)
  {
    _root = root;
    _fragment = fragment;
    Expression = expression;
  }

  protected override Expression Expression { get; }

  public SelectionNodeKind SelectionKind => SelectionNodeKind.FragmentSpread;

  public ReadOnlySpan<char> FragmentName => _fragment.Name;

  public IDirectiveListNode? DirectiveList => Peek(MethodNames.IsDirective) ? this : null;

  public IEnumerable<IDirectiveNode> Directives => this;

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => GetCurrent<IDirectiveNode>();

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator() => GetEnumerator(MethodNames.IsDirective, this);

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodNames.WithDirective => NodeFactory.CreateDirective(_root, methodCall),
      _                         => throw new ExpressionEvaluationException(methodCall)
    };
  }
}
