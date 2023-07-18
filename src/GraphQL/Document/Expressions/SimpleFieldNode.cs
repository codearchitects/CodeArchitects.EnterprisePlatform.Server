using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class SimpleFieldNode : IFieldNode
{
  private readonly string? _alias;
  private readonly string _fieldName;

  public SimpleFieldNode(string? alias, string fieldName)
  {
    _alias = alias;
    _fieldName = fieldName;
  }

  public SelectionNodeKind SelectionKind => SelectionNodeKind.Field;

  public ReadOnlySpan<char> Alias => _alias;

  public ReadOnlySpan<char> FieldName => _fieldName;

  public IArgumentListNode? ArgumentList => null;

  public IDirectiveListNode? DirectiveList => null;

  public ISelectionSetNode? SelectionSet => null;
}
