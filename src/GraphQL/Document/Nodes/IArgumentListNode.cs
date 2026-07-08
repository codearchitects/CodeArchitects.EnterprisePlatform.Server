namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IArgumentListNode
{
  IEnumerable<IArgumentNode> Arguments { get; }
}
