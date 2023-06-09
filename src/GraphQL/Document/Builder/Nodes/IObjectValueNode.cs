namespace CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

internal interface IObjectValueNode
{
  IEnumerable<IObjectFieldNode> Fields { get; }
}
