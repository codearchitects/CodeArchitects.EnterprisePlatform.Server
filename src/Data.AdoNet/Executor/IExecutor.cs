using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal interface IExecutor
{
  Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbCommand command, TKey key, IReadOnlyCollection<string> paths, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
