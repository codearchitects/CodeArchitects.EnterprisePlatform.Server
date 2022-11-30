using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal interface IExecutor
{
  Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbConnection connection, TKey key, NavigationSpec spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
