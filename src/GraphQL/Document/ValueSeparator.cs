using CodeArchitects.Platform.GraphQL.Document.Builder;

namespace CodeArchitects.Platform.GraphQL.Document;

internal abstract class ValueSeparator
{
  private ValueSeparator() { }

  public static readonly ValueSeparator Comma = new CommaSeparator();
  public static readonly ValueSeparator Space = new SpaceSeparator();

  internal abstract void AppendOn(Utf8StringBuilder sb);

  private sealed class CommaSeparator : ValueSeparator
  {
    internal override void AppendOn(Utf8StringBuilder sb)
    {
      sb.AppendComma();
      sb.AppendSpace();
    }
  }

  private sealed class SpaceSeparator : ValueSeparator
  {
    internal override void AppendOn(Utf8StringBuilder sb)
    {
      sb.AppendSpace();
    }
  }
}
