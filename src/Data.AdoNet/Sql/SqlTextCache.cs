using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

internal class SqlTextCache : ISqlTextCache
{
  private readonly IDictionary<NavigationSpec, string> _selectTexts;

  public SqlTextCache(IDictionary<NavigationSpec, string> selectTexts)
  {
    _selectTexts = selectTexts;
  }

  public void AddSelectText(NavigationSpec spec, string text)
  {
    _selectTexts.Add(spec, text);
  }

  public bool TryGetSelectText(NavigationSpec spec, [NotNullWhen(true)] out string? text)
  {
    return _selectTexts.TryGetValue(spec, out text);
  }

  public static SqlTextCache Create()
  {
    return new(new ConcurrentDictionary<NavigationSpec, string>());
  }
}
