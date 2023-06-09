using CodeArchitects.Platform.GraphQL.Document;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.ChillyCream.Document;

internal class ChillyCreamGraphDocument<TResult> : GraphDocument<TResult>
{
  public ChillyCreamGraphDocument(byte[] content)
  {
    Content = content;
  }

  public byte[] Content { get; }

  protected override string GetContent() => Encoding.UTF8.GetString(Content);
}

internal class ChillyCreamGraphDocument<TResult, TVariables> : GraphDocument<TResult, TVariables>
  where TVariables : notnull
{
  public ChillyCreamGraphDocument(byte[] content)
  {
    Content = content;
  }

  public byte[] Content { get; }

  protected override string GetContent() => Encoding.UTF8.GetString(Content);
}
