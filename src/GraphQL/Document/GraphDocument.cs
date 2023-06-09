using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document;

[DebuggerDisplay("{ToString(),raw}")]
internal abstract class GraphDocument<TResult>
{
  protected abstract string GetContent();

  public sealed override string ToString() => GetContent();
}

[DebuggerDisplay("{ToString(),raw}")]
internal abstract class GraphDocument<TResult, TVariables>
  where TVariables : notnull
{
  protected abstract string GetContent();

  public sealed override string ToString() => GetContent();
}
