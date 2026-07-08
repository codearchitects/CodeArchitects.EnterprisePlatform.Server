using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationRoot<TEntity, TKey> : INavigationRoot
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  new IEntityModel<TEntity, TKey> Entity { get; }
}
