namespace CodeArchitects.Platform.GraphQL.ChillyCream.Document.Content;

internal abstract class Symbol
{
  public abstract void Append(Utf8ContentBuilder content);

  public abstract void Append(Utf8ContentBuilder content, int repeatCount);
}
