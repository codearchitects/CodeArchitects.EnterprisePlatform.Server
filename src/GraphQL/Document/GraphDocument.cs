using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document;

[DebuggerDisplay("{ToString(),raw}")]
internal readonly record struct GraphDocument<TResult>(OperationType Type, string Content)
{
  public override string ToString() => Content;
}

[DebuggerDisplay("{ToString(),raw}")]
internal readonly record struct GraphDocument<TResult, TVariables>(OperationType Type, string Content)
  where TVariables : notnull
{
  public override string ToString() => Content;
}
