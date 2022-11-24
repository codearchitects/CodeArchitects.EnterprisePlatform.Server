using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

internal interface ISqlTextCache
{
  bool TryGetSelectText(NavigationCacheKey key, [NotNullWhen(true)] out string? text);
  void AddSelectText(NavigationCacheKey key, string text);
}
