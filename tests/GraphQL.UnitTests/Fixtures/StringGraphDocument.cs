using CodeArchitects.Platform.GraphQL.Document;

namespace CodeArchitects.Platform.GraphQL.Fixtures;

internal class StringGraphDocument<TField> : GraphDocument<TField>
{
  public StringGraphDocument(string content)
  {
    Content = content;
  }

  public string Content { get; }

  protected override string GetContent() => Content;
}


internal class StringGraphDocument<TField, TVariables> : GraphDocument<TField, TVariables>
  where TVariables : notnull
{
  public StringGraphDocument(string content)
  {
    Content = content;
  }

  public string Content { get; }

  protected override string GetContent() => Content;
}
