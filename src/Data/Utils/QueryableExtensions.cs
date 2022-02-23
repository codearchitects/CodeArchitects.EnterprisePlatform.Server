using CodeArchitects.Platform.Common.Collections;
using System.Linq;

namespace CodeArchitects.Platform.Data.Utils;

public static class QueryableExtensions
{
  public static Page<TSource> ToPage<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize)
  {
    TSource[] array = source
      .Skip(pageIndex * pageSize)
      .Take(pageSize + 1)
      .ToArray();

    return array.Length > pageSize
      ? new Page<TSource>(array[0..^1], hasNext: true)
      : new Page<TSource>(array, hasNext: false);
  }
}
