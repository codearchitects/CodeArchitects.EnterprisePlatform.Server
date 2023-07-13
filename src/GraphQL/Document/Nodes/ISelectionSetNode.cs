namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

internal interface ISelectionSetNode
{
  IEnumerable<ISelectionNode> Selections { get; }
}
