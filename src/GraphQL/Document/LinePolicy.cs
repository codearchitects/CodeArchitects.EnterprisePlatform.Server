using System.Text;

namespace CodeArchitects.Platform.GraphQL.Document;

internal abstract class LinePolicy
{
  public static readonly LinePolicy Default = new IndentPolicy(2);

  public static LinePolicy Indent(int indentSize)
  {
    if (indentSize < 0)
      throw new ArgumentOutOfRangeException(nameof(indentSize), indentSize, $"'{nameof(indentSize)}' should not be less than 0.");

    return new IndentPolicy(indentSize);
  }

  public static LinePolicy Space(int spaceSize)
  {
    if (spaceSize < 1)
      throw new ArgumentOutOfRangeException(nameof(spaceSize), spaceSize, $"'{nameof(spaceSize)}' should not be less than 1.");

    return new SpacePolicy(new string(' ', spaceSize));
  }

  internal abstract void AppendLine(StringBuilder sb, int indent);

  private sealed class IndentPolicy : LinePolicy
  {
    private readonly int _indentSize;

    public IndentPolicy(int indentSize)
    {
      _indentSize = indentSize;
    }

    internal override void AppendLine(StringBuilder sb, int indent)
    {
      sb.AppendLine();
      sb.Append(' ', indent * _indentSize);
    }
  }

  private sealed class SpacePolicy : LinePolicy
  {
    private readonly string _space;

    public SpacePolicy(string space)
    {
      _space = space;
    }

    internal override void AppendLine(StringBuilder sb, int indent)
    {
      sb.Append(_space);
    }
  }
}
