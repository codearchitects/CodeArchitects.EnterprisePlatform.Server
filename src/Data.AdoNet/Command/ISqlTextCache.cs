using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ISqlTextCache
{
  bool TryGetSelectText(NavigationSpec spec, [NotNullWhen(true)] out string? text);
  void AddSelectText(NavigationSpec spec, string text);
}
