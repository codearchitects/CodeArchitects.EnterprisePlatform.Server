namespace CodeArchitects.Platform.Common.Collections;

internal static class EnumerableExtensions
{
  public static TResult[] Map<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
  {
    return source.Select(selector).ToArray();
  }
}
