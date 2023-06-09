using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document;

[DebuggerDisplay("{ToString(),raw}")]
internal abstract class GraphDocument<TResult, TVariables> : IGraphDocument<TResult>, IGraphDocument<TResult, TVariables>
  where TVariables : notnull
{
  public abstract override string ToString();
}
