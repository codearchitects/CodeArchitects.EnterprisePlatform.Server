namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

internal interface IDirectiveNode
{
  string Name { get; }

  IEnumerable<IArgumentNode> Arguments { get; }
}
