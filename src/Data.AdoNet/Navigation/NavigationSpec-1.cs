using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal readonly record struct NavigationSpec<TEntity, TKey>(IEntityModel<TEntity, TKey> Entity, IReadOnlyCollection<INavigation> Navigations)
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public static implicit operator NavigationSpec(NavigationSpec<TEntity, TKey> spec) => new NavigationSpec(spec.Entity, spec.Navigations);
}
