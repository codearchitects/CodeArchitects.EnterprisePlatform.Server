namespace CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

internal interface IDirectiveNode
{
  string Name { get; }

  IEnumerable<IArgumentNode> Arguments { get; }
}
