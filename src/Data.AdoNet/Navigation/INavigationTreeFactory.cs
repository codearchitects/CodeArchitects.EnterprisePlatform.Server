using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationTreeFactory
{
  INavigationRoot<TEntity, TKey> Create<TEntity, TKey>(IEntityModel<TEntity, TKey> entityModel, IncludeAction<TEntity> include)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  INavigationRoot<TEntity, TKey> CreateEmpty<TEntity, TKey>(IEntityModel<TEntity, TKey> entityModel)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
