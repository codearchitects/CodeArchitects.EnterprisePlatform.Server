using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Sql.Select;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

internal class SqlTextBuilder : ISqlTextBuilder // TODO: Support multiple database providers
{
  private readonly ISqlTextCache _cache;

  public SqlTextBuilder(ISqlTextCache cache)
  {
    _cache = cache;
  }

  public string BuildSelectText(INavigationRoot root)
  {
    IEntityModel entity = root.Entity;
    IReadOnlyCollection<INavigation> navigations = root.Navigations;
    NavigationCacheKey key = new NavigationCacheKey(entity, navigations);

    if (_cache.TryGetSelectText(key, out string? text))
      return text;

    SelectStringBuilder stringBuilder = new();

    stringBuilder.AppendSelectFrom(entity, navigations);

    foreach (INavigation child in navigations)
    {
      stringBuilder.AppendLeftJoin(child);
    }

    text = stringBuilder.ToString();
    _cache.AddSelectText(key, text);

    return text;
  }
}
