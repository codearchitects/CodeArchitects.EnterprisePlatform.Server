namespace System.Text;

#if !NETCOREAPP2_0_OR_GREATER

internal static partial class StringBuilderExtensions
{
  public static StringBuilder AppendJoin(this StringBuilder stringBuilder, string? separator, params object?[] values)
  {
    separator ??= string.Empty;
    return AppendJoinCore(stringBuilder, separator, values);
  }

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, string? separator, IEnumerable<T> values)
  {
    separator ??= string.Empty;
    return AppendJoinCore(stringBuilder, separator, values);
  }

  public static StringBuilder AppendJoin(this StringBuilder stringBuilder, char separator, params object?[] values)
  {
    return AppendJoinCore(stringBuilder, separator, values);
  }

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, char separator, IEnumerable<T> values)
  {
    return AppendJoinCore(stringBuilder, separator, values);
  }

  private static StringBuilder AppendJoinCore<T>(StringBuilder stringBuilder, char separator, IEnumerable<T> values)
  {
    using (IEnumerator<T> en = values.GetEnumerator())
    {
      if (!en.MoveNext())
        return stringBuilder;

      stringBuilder.Append(en.Current);

      while (en.MoveNext())
      {
        stringBuilder.Append(separator);
        stringBuilder.Append(en.Current);
      }
    }
    return stringBuilder;
  }

  private static StringBuilder AppendJoinCore<T>(StringBuilder stringBuilder, string separator, IEnumerable<T> values)
  {
    using (IEnumerator<T> en = values.GetEnumerator())
    {
      if (!en.MoveNext())
        return stringBuilder;

      stringBuilder.Append(en.Current);

      while (en.MoveNext())
      {
        stringBuilder.Append(separator);
        stringBuilder.Append(en.Current);
      }
    }
    return stringBuilder;
  }
}

#endif
