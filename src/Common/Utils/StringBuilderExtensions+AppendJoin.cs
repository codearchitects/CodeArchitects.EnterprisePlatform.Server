namespace System.Text;

internal delegate void AppendAction<in T>(StringBuilder stringBuilder, T current);
internal delegate void AppendAction<TState, in T>(StringBuilder stringBuilder, ref TState state, T current);

internal static partial class StringBuilderExtensions
{
  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, string separator, IEnumerable<T> values, AppendAction<T> append)
  {
    return AppendJoinCore(stringBuilder, separator, values, append);
  }

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, char separator, IEnumerable<T> values, AppendAction<T> append)
  {
    return AppendJoinCore(stringBuilder, separator, values, append);
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, string separator, ref TState state, IEnumerable<T> values, AppendAction<TState, T> append)
  {
    return AppendJoinCore(stringBuilder, separator, ref state, values, append);
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, char separator, ref TState state, IEnumerable<T> values, AppendAction<TState, T> append)
  {
    return AppendJoinCore(stringBuilder, separator, ref state, values, append);
  }

  private static StringBuilder AppendJoinCore<T>(StringBuilder stringBuilder, char separator, IEnumerable<T> values, AppendAction<T> append)
  {
    using IEnumerator<T> en = values.GetEnumerator();

    if (!en.MoveNext())
      return stringBuilder;

    append(stringBuilder, en.Current);

    while (en.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, en.Current);
    }

    return stringBuilder;
  }

  private static StringBuilder AppendJoinCore<T>(StringBuilder stringBuilder, string separator, IEnumerable<T> values, AppendAction<T> append)
  {
    using IEnumerator<T> en = values.GetEnumerator();

    if (!en.MoveNext())
      return stringBuilder;

    append(stringBuilder, en.Current);

    while (en.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, en.Current);
    }

    return stringBuilder;
  }

  private static StringBuilder AppendJoinCore<TState, T>(StringBuilder stringBuilder, char separator, ref TState state, IEnumerable<T> values, AppendAction<TState, T> append)
  {
    using IEnumerator<T> en = values.GetEnumerator();

    if (!en.MoveNext())
      return stringBuilder;

    append(stringBuilder, ref state, en.Current);

    while (en.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, ref state, en.Current);
    }

    return stringBuilder;
  }

  private static StringBuilder AppendJoinCore<TState, T>(StringBuilder stringBuilder, string separator, ref TState state, IEnumerable<T> values, AppendAction<TState, T> append)
  {
    using IEnumerator<T> en = values.GetEnumerator();

    if (!en.MoveNext())
      return stringBuilder;

    append(stringBuilder, ref state, en.Current);

    while (en.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, ref state, en.Current);
    }

    return stringBuilder;
  }
}

#if !NETCOREAPP2_0_OR_GREATER

internal static partial class StringBuilderExtensions
{
  public static StringBuilder AppendJoin(this StringBuilder stringBuilder, string separator, params object?[] values)
  {
    return AppendJoinCore(stringBuilder, separator, values, static (sb, current) => sb.Append(current));
  }

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, string separator, IEnumerable<T> values)
  {
    return AppendJoinCore(stringBuilder, separator, values, static (sb, current) => sb.Append(current));
  }

  public static StringBuilder AppendJoin(this StringBuilder stringBuilder, char separator, params object?[] values)
  {
    return AppendJoinCore(stringBuilder, separator, values, static (sb, current) => sb.Append(current));
  }

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, char separator, IEnumerable<T> values)
  {
    return AppendJoinCore(stringBuilder, separator, values, static (sb, current) => sb.Append(current));
  }
}

#endif
