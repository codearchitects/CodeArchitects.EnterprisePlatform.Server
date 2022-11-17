using CodeArchitects.Platform.Data.AdoNet.Commands;
using CodeArchitects.Platform.Data.AdoNet.Materialization;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal class Executor<TEntity, TKey> : IExecutor<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private readonly ICommandBuilder<TEntity, TKey> _builder;
  private readonly IMaterializer<TEntity> _materializer;

  public Executor(ICommandBuilder<TEntity, TKey> builder, IMaterializer<TEntity> materializer)
  {
    _builder = builder;
    _materializer = materializer;
  }

  public async Task<TEntity?> ExecuteSelectCommandAsync(DbCommand command, TKey key, IReadOnlyCollection<string> paths, CancellationToken cancellationToken)
  {
    _builder.BuildSelectCommand(command, key, paths);

    DbDataReader dataReader = await command.ExecuteReaderAsync(cancellationToken);

    return await _materializer.MaterializeAsync(dataReader);
  }
}
