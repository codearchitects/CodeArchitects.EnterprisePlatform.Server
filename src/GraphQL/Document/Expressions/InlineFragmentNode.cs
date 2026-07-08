using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal abstract class InlineFragmentNode : IteratorNode, IInlineFragmentNode,
  IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  private readonly INodeRoot _root;

  protected InlineFragmentNode(INodeRoot root, Expression expression)
  {
    _root = root;
    Expression = expression;
  }

  public abstract ITypeConditionNode? TypeCondition { get; }

  protected sealed override Expression Expression { get; }

  public SelectionNodeKind SelectionKind => SelectionNodeKind.InlineFragment;

  public IDirectiveListNode? DirectiveList => Peek(MethodNames.IsDirective) ? this : null;

  public ISelectionSetNode SelectionSet => GetNext<ISelectionSetNode>(MethodNames.IsSelectionSet);

  public IEnumerable<IDirectiveNode> Directives => this;

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
