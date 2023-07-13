namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

internal interface IArgumentNode
{
  string Name { get; }

  object? Value { get; }
}