namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IListValueNode
{
  IEnumerable<object?> Values { get; }
}
