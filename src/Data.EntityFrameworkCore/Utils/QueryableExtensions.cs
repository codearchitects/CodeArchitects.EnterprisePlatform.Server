using CodeArchitects.Platform.Common.Collections;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Utils;

public static class QueryableExtensions
{
  public static async Task<Page<TSource>> ToPageAsync<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
  {
    TSource[] array = await source
      .Skip(pageIndex * pageSize)
      .Take(pageSize + 1)
      .ToArrayAsync(cancellationToken);

    return array.Length > pageSize
      ? new Page<TSource>(array[0..^1], hasNext: true)
      : new Page<TSource>(array, hasNext: false);
  }
}
