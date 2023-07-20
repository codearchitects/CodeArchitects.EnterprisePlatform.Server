namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IBlockStringValueNode : IValueNode
{
  IEnumerable<string> Lines { get; }
}
