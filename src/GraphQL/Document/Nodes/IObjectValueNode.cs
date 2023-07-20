namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IObjectValueNode : IValueNode
{
  IEnumerable<IObjectFieldNode> Fields { get; }
}
