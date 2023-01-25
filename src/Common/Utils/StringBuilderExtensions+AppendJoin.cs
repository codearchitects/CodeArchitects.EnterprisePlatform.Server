namespace System.Text;

internal delegate void AppendAction<in T>(StringBuilder stringBuilder, T current);
internal delegate void AppendAction<TState, in T>(StringBuilder stringBuilder, TState state, T current);

internal static partial class StringBuilderExtensions
{
  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, string separator, IEnumerable<T> values, AppendAction<T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, enumerator.Current);

    while (enumerator.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, char separator, IEnumerable<T> values, AppendAction<T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, enumerator.Current);

    while (enumerator.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, string separator, in TState state, IEnumerable<T> values, AppendAction<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, state, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, char separator, in TState state, IEnumerable<T> values, AppendAction<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, state, enumerator.Current);
    }

    return stringBuilder;
  }
}

#if !NETCOREAPP2_0_OR_GREATER

internal static partial class StringBuilderExtensions
{
  public static StringBuilder AppendJoin(this StringBuilder stringBuilder, string separator, params object?[] values)
  {
    return AppendJoin(stringBuilder, separator, values, static (stringBuilder, value) => stringBuilder.Append(value));
  }

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, string separator, IEnumerable<T> values)
  {
    return AppendJoin(stringBuilder, separator, values, static (stringBuilder, value) => stringBuilder.Append(value));
  }

  public static StringBuilder AppendJoin(this StringBuilder stringBuilder, char separator, params object?[] values)
  {
    return AppendJoin(stringBuilder, separator, values, static (stringBuilder, value) => stringBuilder.Append(value));
  }

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, char separator, IEnumerable<T> values)
  {
    return AppendJoin(stringBuilder, separator, values, static (stringBuilder, value) => stringBuilder.Append(value));
  }
}

#endif
