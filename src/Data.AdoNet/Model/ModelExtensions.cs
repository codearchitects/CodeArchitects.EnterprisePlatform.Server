using CodeArchitects.Platform.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public static class ModelExtensions
{
  public static bool TryGetNavigation(this IEntityModel entity, string name, [NotNullWhen(true)] out IAccessibleNavigationModel? navigation)
    => entity.TryGetNavigation(name.AsSpan(), out navigation);
}
