using CodeArchitects.Platform.Data.AdoNet.Commands;
using CodeArchitects.Platform.Data.AdoNet.Materialization;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal class Executor<TEntity, TKey> : IExecutor<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public async Task<TEntity?> ExecuteSelectCommandAsync(DbCommand command, TKey key, IReadOnlyCollection<string> paths, CancellationToken cancellationToken)
  {
    // _builder.BuildSelectCommand(command, key, paths);

    DbDataReader dataReader = await command.ExecuteReaderAsync(cancellationToken);

    return null;
    // return await _materializer.MaterializeAsync(dataReader);
  }
}
