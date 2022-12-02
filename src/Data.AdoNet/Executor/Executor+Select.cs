using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor
{
  public Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbConnection connection, TKey key, in NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    DbCommand command = connection.CreateCommand();
    _commandBuilder.BuildSelectCommand(command, key, in spec);

    return ExecuteSelectCommandAsync(command, spec, cancellationToken);
  }

  private async Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbCommand command, NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    using (command)
    {
      DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

      return await _materializer.ReadEntityAsync(reader, spec, cancellationToken);
    }
  }
}
