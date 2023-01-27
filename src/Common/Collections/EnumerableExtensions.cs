using System.Linq;

namespace CodeArchitects.Platform.Common.Collections;

internal static class EnumerableExtensions
{
  public static TResult[] Map<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
  {
    return source.Select(selector).ToArray();
  }

  public static TResult[] Map<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, TResult> selector)
  {
    if (source.Count == 0)
      return Array.Empty<TResult>();

    TResult[] array = new TResult[source.Count];
    for (int i = 0; i < source.Count; i++)
    {
      array[i] = selector(source[i]);
    }

    return array;
  }

  public static TResult[] Map<TState, TSource, TResult>(this IReadOnlyList<TSource> source, TState state, Func<TState, TSource, TResult> selector)
  {
    if (source.Count == 0)
      return Array.Empty<TResult>();

    TResult[] array = new TResult[source.Count];
    for (int i = 0; i < source.Count; i++)
    {
      array[i] = selector(state, source[i]);
    }

    return array;
  }
}
