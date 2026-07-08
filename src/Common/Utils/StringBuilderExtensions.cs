namespace System.Text;

internal static partial class StringBuilderExtensions
{
  public static StringBuilder AppendCamelized(this StringBuilder stringBuilder, string value)
  {
    if (value == string.Empty)
      return stringBuilder;

    stringBuilder.Append(char.ToLower(value[0]));
    stringBuilder.Append(value.AsSpan()[1..]);

    return stringBuilder;
  }
}
