namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document.Content;

internal abstract class Symbol
{
  public abstract void Append(Utf8ContentBuilder content);

  public abstract void Append(Utf8ContentBuilder content, int repeatCount);
}
