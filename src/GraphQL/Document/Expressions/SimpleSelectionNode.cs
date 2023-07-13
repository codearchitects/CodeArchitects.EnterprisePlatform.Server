using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class SimpleSelectionNode : ISelectionNode
{
  public SimpleSelectionNode(string? alias, string fieldName)
  {
    Alias = alias;
    FieldName = fieldName;
  }

  public string? Alias { get; }

  public string FieldName { get; }

  public IEnumerable<IArgumentNode> Arguments => Enumerable.Empty<IArgumentNode>();

  public IEnumerable<IDirectiveNode> Directives => Enumerable.Empty<IDirectiveNode>();

  public ISelectionSetNode? SelectionSet => null;
}
