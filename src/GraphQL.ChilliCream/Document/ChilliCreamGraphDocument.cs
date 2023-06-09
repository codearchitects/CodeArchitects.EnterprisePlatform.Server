using CodeArchitects.Platform.GraphQL.Document;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

internal class ChilliCreamGraphDocument<TResult, TVariables> : GraphDocument<TResult, TVariables>
  where TVariables : notnull
{
  public ChilliCreamGraphDocument(byte[] content)
  {
    Content = content;
  }

  public byte[] Content { get; }

  public override string ToString() => Encoding.UTF8.GetString(Content);
}
