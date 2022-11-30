using CodeArchitects.Platform.Data.AdoNet.Materialization;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Sql;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal class Executor : IExecutor
{
  private readonly ISqlTextBuilder _sqlBuilder;
  private readonly IMaterializer _materializer;

  public Executor(ISqlTextBuilder sqlBuilder, IMaterializer materializer)
  {
    _sqlBuilder = sqlBuilder;
    _materializer = materializer;
  }

  public async Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbConnection connection, TKey key, NavigationSpec spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    using DbCommand command = connection.CreateCommand();

    command.CommandText = _sqlBuilder.BuildSelectText(spec);
    // Set parameters

    DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
    return await _materializer.ReadEntityAsync<TEntity, TKey>(reader, spec, cancellationToken);
  }

  public async Task ExecuteInsertCommandAsync<TEntity, TKey>(DbConnection connection, TEntity entity, IEntityModel model, CancellationToken cancellationToken)
  {
    using DbCommand command = connection.CreateCommand();

    // command.CommandText = _sqlBuilder.BuildInsertText(model);
    // Set parameters

    object? result = await command.ExecuteScalarAsync(cancellationToken);
  }
}
