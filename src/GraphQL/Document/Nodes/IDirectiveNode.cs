namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IDirectiveNode
{
  ReadOnlySpan<char> Name { get; }

  IArgumentListNode? ArgumentList { get; }
}
