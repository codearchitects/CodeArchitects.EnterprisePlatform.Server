using CodeArchitects.Platform.GraphQL.Document;

namespace CodeArchitects.Platform.GraphQL.Fixtures;

internal class StringGraphDocument<TField, TVariables> : GraphDocument<TField, TVariables>
  where TVariables : notnull
{
  public StringGraphDocument(string content)
  {
    Content = content;
  }

  public string Content { get; }

  public override string ToString() => Content;
}
