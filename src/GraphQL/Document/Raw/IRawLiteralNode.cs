namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal interface IRawLiteralNode
{
  ReadOnlySpan<char> ValueText { get; }
}
