using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Fixtures;

internal record FakeNavigationRoot<TEntity, TKey>(IEntityModel<TEntity, TKey> Entity, IReadOnlyCollection<INavigation> Navigations) : INavigationRoot<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  IEntityModel INavigationRoot.Entity => Entity;
}