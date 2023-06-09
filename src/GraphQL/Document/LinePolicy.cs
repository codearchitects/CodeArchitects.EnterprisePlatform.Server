using CodeArchitects.Platform.GraphQL.Document.Builder.Content;

namespace CodeArchitects.Platform.GraphQL.Document;

internal abstract class LinePolicy
{
  public static readonly LinePolicy Default = new IndentPolicy(2);

  public static LinePolicy Indent(int indentSize) => new IndentPolicy(indentSize);

  public static LinePolicy Space(int spaceSize) => new SpacePolicy(spaceSize);

  internal abstract void Append<TSymbol>(IContentBuilder<TSymbol> content, int indent);

  private sealed class IndentPolicy : LinePolicy
  {
    private readonly int _indentSize;

    public IndentPolicy(int indentSize)
    {
      if (indentSize < 0)
        throw new ArgumentOutOfRangeException(nameof(indentSize), indentSize, $"'{nameof(indentSize)}' should not be less than 0.");

      _indentSize = indentSize;
    }

    internal override void Append<TSymbol>(IContentBuilder<TSymbol> content, int indent)
    {
      content.AppendLine();
      content.Append(content.Trivias.Space, indent * _indentSize);
    }
  }

  private sealed class SpacePolicy : LinePolicy
  {
    private readonly int _spaceSize;

    public SpacePolicy(int spaceSize)
    {
      if (spaceSize < 1)
        throw new ArgumentOutOfRangeException(nameof(spaceSize), spaceSize, $"'{nameof(spaceSize)}' should not be less than 1.");

      _spaceSize = spaceSize;
    }

    internal override void Append<TSymbol>(IContentBuilder<TSymbol> content, int indent)
    {
      content.Append(content.Trivias.Space, _spaceSize);
    }
  }
}
