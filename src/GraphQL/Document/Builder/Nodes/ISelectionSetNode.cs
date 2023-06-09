namespace CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

internal interface ISelectionSetNode
{
  IEnumerable<ISelectionNode> Selections { get; }
}
