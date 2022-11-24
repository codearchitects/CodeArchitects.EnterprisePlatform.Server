using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

internal class SqlTextCache : ISqlTextCache
{
  private readonly IDictionary<NavigationCacheKey, string> _selectTexts;

  public SqlTextCache(IDictionary<NavigationCacheKey, string> selectTexts)
  {
    _selectTexts = selectTexts;
  }

  public void AddSelectText(NavigationCacheKey key, string text)
  {
    _selectTexts.Add(key, text);
  }

  public bool TryGetSelectText(NavigationCacheKey key, [NotNullWhen(true)] out string? text)
  {
    return _selectTexts.TryGetValue(key, out text);
  }

  public static SqlTextCache Create()
  {
    return new(new ConcurrentDictionary<NavigationCacheKey, string>());
  }
}
