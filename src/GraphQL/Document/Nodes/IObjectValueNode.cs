namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IObjectValueNode
{
  IEnumerable<IObjectFieldNode> Fields { get; }
}
