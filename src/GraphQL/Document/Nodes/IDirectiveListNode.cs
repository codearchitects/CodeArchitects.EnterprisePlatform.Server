namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IDirectiveListNode
{
  IEnumerable<IDirectiveNode> Directives { get; }
}
