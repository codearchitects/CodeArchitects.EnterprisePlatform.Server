using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

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
