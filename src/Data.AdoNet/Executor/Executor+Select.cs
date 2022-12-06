using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor
{
  public Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbCommand command, TKey key, in NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _commandBuilder.BuildSelectCommand(command, key, in spec);

    return ExecuteSelectCommandAsync(command, spec, cancellationToken);
  }

  private async Task<TEntity?> ExecuteSelectCommandAsync<TEntity, TKey>(DbCommand command, NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

    return await _materializer.ReadEntityAsync(reader, spec, cancellationToken);
  }
}
