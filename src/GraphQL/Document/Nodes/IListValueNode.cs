namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IListValueNode : IValueNode
{
  IEnumerable<IValueNode> Values { get; }
}
