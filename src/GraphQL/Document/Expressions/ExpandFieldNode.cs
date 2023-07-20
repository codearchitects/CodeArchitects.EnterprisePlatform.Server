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

  public IArgumentListNode? ArgumentList => Peek(MethodName.Represents.Argument) ? this : null;

  public IEnumerable<IArgumentNode> Arguments => this;

  public IDirectiveListNode? DirectiveList => Peek(MethodName.Represents.Directive) ? this : null;

  public IEnumerable<IDirectiveNode> Directives => this;

  IArgumentNode IEnumerator<IArgumentNode>.Current => GetCurrent<IArgumentNode>();

  IEnumerator<IArgumentNode> IEnumerable<IArgumentNode>.GetEnumerator() => GetEnumerator(MethodName.Represents.Argument, this);

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => GetCurrent<IDirectiveNode>();

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator() => GetEnumerator(MethodName.Represents.Directive, this);

  public ISelectionSetNode? SelectionSet
  {
    get
    {
      if (TryGetNext(MethodName.Represents.SelectionSet, out ISelectionSetNode? selectionSet))
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
      MethodName.WithArgument     => NodeFactory.CreateArgument(_root, methodCall),
      MethodName.WithDirective    => NodeFactory.CreateDirective(_root, methodCall),
      MethodName.WithSelection    => NodeFactory.CreateSimpleSelectionSet(_root, methodCall),
      MethodName.WithSelectionSet => NodeFactory.CreateSelectionSet(_root, methodCall),
      _                           => throw new ExpressionEvaluationException(methodCall)
    };
  }
}
