using CodeArchitects.Platform.Common.Collections;

namespace System.Text;

internal delegate void AppendSeparator(StringBuilder stringBuilder);
internal delegate void StatefulAppendSeparator<TState>(StringBuilder stringBuilder, TState state);
internal delegate void InStatefulAppendSeparator<TState>(StringBuilder stringBuilder, in TState state);
internal delegate void RefStatefulAppendSeparator<TState>(StringBuilder stringBuilder, ref TState state);
internal delegate void Append<in T>(StringBuilder stringBuilder, T current);
internal delegate void Append<in T1, in T2>(StringBuilder stringBuilder, T1 current1, T2 current2);
internal delegate void StatefulAppend<TState, in T>(StringBuilder stringBuilder, TState state, T current);
internal delegate void StatefulAppend<TState, in T1, in T2>(StringBuilder stringBuilder, TState state, T1 current1, T2 current2);
internal delegate void InStatefulAppend<TState, in T>(StringBuilder stringBuilder, in TState state, T current);
internal delegate void InStatefulAppend<TState, in T1, in T2>(StringBuilder stringBuilder, in TState state, T1 current1, T2 current2);
internal delegate void RefStatefulAppend<TState, in T>(StringBuilder stringBuilder, ref TState state, T current);
internal delegate void RefStatefulAppend<TState, in T1, in T2>(StringBuilder stringBuilder, ref TState state, T1 current1, T2 current2);

internal static partial class StringBuilderExtensions
{
  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, string separator, IEnumerable<T> values, Append<T> append)
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

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, char separator, IEnumerable<T> values, Append<T> append)
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

  public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, AppendSeparator appendSeparator, IEnumerable<T> values, Append<T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, enumerator.Current);

    while (enumerator.MoveNext())
    {
      appendSeparator(stringBuilder);
      append(stringBuilder, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<T1, T2>(this StringBuilder stringBuilder, string separator, IEnumerable<T1> values1, IEnumerable<T2> values2, Append<T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      stringBuilder.Append(separator);
      append(stringBuilder, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<T1, T2>(this StringBuilder stringBuilder, char separator, IEnumerable<T1> values1, IEnumerable<T2> values2, Append<T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      stringBuilder.Append(separator);
      append(stringBuilder, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<T1, T2>(this StringBuilder stringBuilder, AppendSeparator appendSeparator, IEnumerable<T1> values1, IEnumerable<T2> values2, Append<T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      appendSeparator(stringBuilder);
      append(stringBuilder, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, string separator, TState state, IEnumerable<T> values, StatefulAppend<TState, T> append)
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

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, string separator, in TState state, IEnumerable<T> values, InStatefulAppend<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, in state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, in state, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, string separator, ref TState state, IEnumerable<T> values, RefStatefulAppend<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, ref state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, ref state, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, char separator, TState state, IEnumerable<T> values, StatefulAppend<TState, T> append)
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

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, char separator, in TState state, IEnumerable<T> values, InStatefulAppend<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, in state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, in state, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, char separator, ref TState state, IEnumerable<T> values, RefStatefulAppend<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, ref state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      stringBuilder.Append(separator);
      append(stringBuilder, ref state, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, StatefulAppendSeparator<TState> appendSeparator, TState state, IEnumerable<T> values, StatefulAppend<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      appendSeparator(stringBuilder, state);
      append(stringBuilder, state, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, InStatefulAppendSeparator<TState> appendSeparator, in TState state, IEnumerable<T> values, InStatefulAppend<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, in state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      appendSeparator(stringBuilder, in state);
      append(stringBuilder, in state, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T>(this StringBuilder stringBuilder, RefStatefulAppendSeparator<TState> appendSeparator, ref TState state, IEnumerable<T> values, RefStatefulAppend<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return stringBuilder;

    append(stringBuilder, ref state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      appendSeparator(stringBuilder, ref state);
      append(stringBuilder, ref state, enumerator.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T1, T2>(this StringBuilder stringBuilder, string separator, TState state, IEnumerable<T1> values1, IEnumerable<T2> values2, StatefulAppend<TState, T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, state, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      stringBuilder.Append(separator);
      append(stringBuilder, state, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T1, T2>(this StringBuilder stringBuilder, string separator, in TState state, IEnumerable<T1> values1, IEnumerable<T2> values2, InStatefulAppend<TState, T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, in state, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      stringBuilder.Append(separator);
      append(stringBuilder, in state, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T1, T2>(this StringBuilder stringBuilder, string separator, ref TState state, IEnumerable<T1> values1, IEnumerable<T2> values2, RefStatefulAppend<TState, T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, ref state, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      stringBuilder.Append(separator);
      append(stringBuilder, ref state, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T1, T2>(this StringBuilder stringBuilder, char separator, TState state, IEnumerable<T1> values1, IEnumerable<T2> values2, StatefulAppend<TState, T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, state, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      stringBuilder.Append(separator);
      append(stringBuilder, state, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T1, T2>(this StringBuilder stringBuilder, char separator, in TState state, IEnumerable<T1> values1, IEnumerable<T2> values2, InStatefulAppend<TState, T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, in state, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      stringBuilder.Append(separator);
      append(stringBuilder, in state, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T1, T2>(this StringBuilder stringBuilder, char separator, ref TState state, IEnumerable<T1> values1, IEnumerable<T2> values2, RefStatefulAppend<TState, T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, ref state, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      stringBuilder.Append(separator);
      append(stringBuilder, ref state, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T1, T2>(this StringBuilder stringBuilder, StatefulAppendSeparator<TState> appendSeparator, TState state, IEnumerable<T1> values1, IEnumerable<T2> values2, StatefulAppend<TState, T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, state, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      appendSeparator(stringBuilder, state);
      append(stringBuilder, state, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T1, T2>(this StringBuilder stringBuilder, InStatefulAppendSeparator<TState> appendSeparator, in TState state, IEnumerable<T1> values1, IEnumerable<T2> values2, InStatefulAppend<TState, T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, in state, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      appendSeparator(stringBuilder, in state);
      append(stringBuilder, in state, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }

  public static StringBuilder AppendJoin<TState, T1, T2>(this StringBuilder stringBuilder, RefStatefulAppendSeparator<TState> appendSeparator, ref TState state, IEnumerable<T1> values1, IEnumerable<T2> values2, RefStatefulAppend<TState, T1, T2> append)
  {
    using IEnumerator<T1> enumerator1 = values1.GetEnumerator();
    using IEnumerator<T2> enumerator2 = values2.GetEnumerator();

    if (!Enumerate.MoveNext(enumerator1, enumerator2))
      return stringBuilder;

    append(stringBuilder, ref state, enumerator1.Current, enumerator2.Current);

    while (Enumerate.MoveNext(enumerator1, enumerator2))
    {
      appendSeparator(stringBuilder, ref state);
      append(stringBuilder, ref state, enumerator1.Current, enumerator2.Current);
    }

    return stringBuilder;
  }
}
