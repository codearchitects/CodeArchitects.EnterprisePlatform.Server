using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class FieldNode : IFieldNode
{
  private readonly INodeContext _context;
  private readonly LambdaExpression _expression;
  private readonly string? _alias;
  private readonly string _fieldName;

  public FieldNode(INodeContext context, string? alias, string fieldName, LambdaExpression expression)
  {
    _context = context;
    _alias = alias;
    _fieldName = fieldName;
    _expression = expression;
  }

  public SelectionNodeKind SelectionKind => SelectionNodeKind.Field;

  public ReadOnlySpan<char> Alias => _alias;

  public ReadOnlySpan<char> FieldName => _fieldName;

  public IArgumentListNode? ArgumentList => null;

  public IDirectiveListNode? DirectiveList => null;

  public ISelectionSetNode? SelectionSet => new NamedSelectionSetNode(_context, _expression.Parameters[0], (MemberInitExpression)_expression.Body);
}
