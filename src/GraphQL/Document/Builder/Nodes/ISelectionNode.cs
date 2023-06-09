namespace CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

internal interface ISelectionNode
{
  string? Alias { get; }

  string FieldName { get; }

  IEnumerable<IArgumentNode> Arguments { get; }

  IEnumerable<IDirectiveNode> Directives { get; }

  ISelectionSetNode? SelectionSet { get; }
}
