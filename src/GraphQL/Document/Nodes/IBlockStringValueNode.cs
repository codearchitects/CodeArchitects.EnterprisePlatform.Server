namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IBlockStringValueNode : IValueNode
{
  IEnumerable<ReadOnlyMemory<char>> Lines { get; }
}
