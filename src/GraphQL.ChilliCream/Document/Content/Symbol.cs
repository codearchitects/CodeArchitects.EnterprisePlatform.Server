namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document.Content;

internal abstract class Symbol
{
  public abstract void AppendOn(Utf8ContentBuilder content);

  public abstract void AppendOn(Utf8ContentBuilder content, int repeatCount);
}
