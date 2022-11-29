using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal static class ModelExtensions
{
  public static bool TryGetNavigation(this IEntityModel entity, string name, [NotNullWhen(true)] out INavigationModelBase? navigation)
    => entity.TryGetNavigation(name.AsSpan(), out navigation);
}
