using CodeArchitects.Platform.GraphQL.Document.Content;

namespace CodeArchitects.Platform.GraphQL.Document;

internal abstract class ValueSeparator
{
  public static readonly ValueSeparator Comma = new CommaSeparator();
  public static readonly ValueSeparator Space = new SpaceSeparator();

  internal abstract void Append<TSymbol>(IContentBuilder<TSymbol> content);

  private sealed class CommaSeparator : ValueSeparator
  {
    internal override void Append<TSymbol>(IContentBuilder<TSymbol> content)
    {
      content.Append(content.Trivias.Comma);
      content.Append(content.Trivias.Space);
    }
  }

  private sealed class SpaceSeparator : ValueSeparator
  {
    internal override void Append<TSymbol>(IContentBuilder<TSymbol> content)
    {
      content.Append(content.Trivias.Space);
    }
  }
}
