using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ExpandFieldNode : IteratorNode, IFieldNode,
  IArgumentListNode, IEnumerable<IArgumentNode>, IEnumerator<IArgumentNode>,
  IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  private readonly INodeRoot _root;
  private readonly string? _alias;
  private readonly string _fieldName;

  public ExpandFieldNode(INodeRoot root, string? alias, string fieldName, Expression expression)
  {
    _root = root;
    _alias = alias;
    _fieldName = fieldName;
    Expression = expression;
  }

  protected override Expression Expression { get; }

  public SelectionNodeKind SelectionKind => SelectionNodeKind.Field;

  public ReadOnlySpan<char> Alias => _alias;

  public ReadOnlySpan<char> FieldName => _fieldName;

  public IArgumentListNode? ArgumentList => Peek(MethodNames.IsArgument) ? this : null;

  public IEnumerable<IArgumentNode> Arguments => this;

  public IDirectiveListNode? DirectiveList => Peek(MethodNames.IsDirective) ? this : null;

  public IEnumerable<IDirectiveNode> Directives => this;

  IArgumentNode IEnumerator<IArgumentNode>.Current => GetCurrent<IArgumentNode>();

  IEnumerator<IArgumentNode> IEnumerable<IArgumentNode>.GetEnumerator() => GetEnumerator(MethodNames.IsArgument, this);

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => GetCurrent<IDirectiveNode>();

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator() => GetEnumerator(MethodNames.IsDirective, this);

  public ISelectionSetNode? SelectionSet
  {
    get
    {
      if (TryGetNext(MethodNames.IsSelectionSet, out ISelectionSetNode? selectionSet))
        return selectionSet;

      Type fieldType = Expression.Type.GetGenericArguments()[0];
      if (_root.Context.TryGetDefaultSelection(fieldType, out LambdaExpression? defaultSelection))
        return new NamedSimpleSelectionSetNode(_root, defaultSelection.Parameters[0], (MemberInitExpression)defaultSelection.Body);

      return null;
    }
  }

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodNames.WithArgument     => NodeFactory.CreateArgument(_root, methodCall),
      MethodNames.WithDirective    => NodeFactory.CreateDirective(_root, methodCall),
      MethodNames.WithSelection    => NodeFactory.CreateSimpleSelectionSet(_root, methodCall),
      MethodNames.WithSelectionSet => NodeFactory.CreateSelectionSet(_root, methodCall),
      _                            => throw new ExpressionEvaluationException(methodCall)
    };
  }
}
