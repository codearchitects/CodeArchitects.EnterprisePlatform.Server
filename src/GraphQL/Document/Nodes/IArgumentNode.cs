namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IArgumentNode
{
  ReadOnlySpan<char> Name { get; }

  object? Value { get; }
}