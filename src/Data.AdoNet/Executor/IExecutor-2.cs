using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal interface IExecutor<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  Task<TEntity?> ExecuteSelectCommandAsync(DbCommand command, TKey key, IReadOnlyCollection<string> paths, CancellationToken cancellationToken);
}
