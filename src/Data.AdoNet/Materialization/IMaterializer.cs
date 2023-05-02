using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializer
{
  TEntity? ReadEntity<TEntity, TKey>(DbDataReader reader, INavigationRoot<TEntity, TKey> root)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  Task<TEntity?> ReadEntityAsync<TEntity, TKey>(DbDataReader reader, INavigationRoot<TEntity, TKey> root, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
