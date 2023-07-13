namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

internal interface IObjectValueNode
{
  IEnumerable<IObjectFieldNode> Fields { get; }
}
