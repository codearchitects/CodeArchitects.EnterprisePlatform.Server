using CodeArchitects.Platform.Common.Collections;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.Utils
{
  public static class QueryableExtensions
  {
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
    {
      return condition
        ? source.Where(predicate)
        : source;
    }

    public static Page<TSource> ToPage<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize)
    {
      TSource[] array = source
        .Skip(pageIndex * pageSize)
        .Take(pageSize + 1)
        .ToArray();

      return array.Length > pageSize
        ? new Page<TSource>(array[0..^1], hasNext: true)
        : new Page<TSource>(array,        hasNext: false);
    }
  }
}
