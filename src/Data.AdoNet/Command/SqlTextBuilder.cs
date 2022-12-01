using CodeArchitects.Platform.Data.AdoNet.Command.Select;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal class SqlTextBuilder : ISqlTextBuilder
{
  private readonly ISqlTextCache _cache;

  public SqlTextBuilder(ISqlTextCache cache)
  {
    _cache = cache;
  }

  public string BuildSelectText(NavigationSpec spec)
  {
    if (_cache.TryGetSelectText(spec, out string? text))
      return text;

    SelectStringBuilder stringBuilder = new();

    stringBuilder.AppendSelectFrom(spec.Entity, spec.Navigations);

    foreach (INavigation child in spec.Navigations)
    {
      stringBuilder.AppendLeftJoin(child);
    }

    text = stringBuilder.ToString();
    _cache.AddSelectText(spec, text);

    return text;
  }
}
