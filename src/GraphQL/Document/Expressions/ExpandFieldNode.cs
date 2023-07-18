using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Builder;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ExpandFieldNode : IteratorNode, IFieldNode,
  IArgumentListNode, IEnumerable<IArgumentNode>, IEnumerator<IArgumentNode>,
  IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  private readonly INodeContext _context;
  private readonly string? _alias;
  private readonly string _fieldName;

  public ExpandFieldNode(INodeContext context, string? alias, string fieldName, Expression expression)
  {
    _context = context;
    _alias = alias;
    _fieldName = fieldName;
    Expression = expression;
  }

  protected override Expression Expression { get; }

  public SelectionNodeKind SelectionKind => SelectionNodeKind.Field;

  public ReadOnlySpan<char> Alias => _alias;

  public ReadOnlySpan<char> FieldName => _fieldName;

  public IArgumentListNode? ArgumentList => Peek(MethodNames.WithArgument) ? this : null;

  public IEnumerable<IArgumentNode> Arguments => this;

  public IDirectiveListNode? DirectiveList => Peek(MethodNames.WithDirective) ? this : null;

  public IEnumerable<IDirectiveNode> Directives => this;

  IArgumentNode IEnumerator<IArgumentNode>.Current => GetCurrent<IArgumentNode>();

  IEnumerator<IArgumentNode> IEnumerable<IArgumentNode>.GetEnumerator() => GetEnumerator(MethodNames.WithArgument, this);

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => GetCurrent<IDirectiveNode>();

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator() => GetEnumerator(MethodNames.WithDirective, this);

  public ISelectionSetNode? SelectionSet
  {
    get
    {
      if (TryGetNext(MethodNames.WithSelection, out ISelectionSetNode? selectionSet))
        return selectionSet;

      Type fieldType = Expression.Type.GetGenericArguments()[0];
      if (_context.TryGetDefaultSelection(fieldType, out LambdaExpression? defaultSelection))
        return new NamedSelectionSetNode(_context, defaultSelection.Parameters[0], (MemberInitExpression)defaultSelection.Body);

      return null;
    }
  }

  protected override object OnMethodCall(MethodCallExpression methodCall)
  {
    return methodCall.Method.Name switch
    {
      MethodNames.WithArgument  => NodeFactory.CreateArgument(_context, methodCall),
      MethodNames.WithDirective => NodeFactory.CreateDirective(_context, methodCall),
      MethodNames.WithSelection => NodeFactory.CreateSelectionSet(_context, methodCall),
      _                         => throw new ExpressionEvaluationException(methodCall)
    };
  }
}
