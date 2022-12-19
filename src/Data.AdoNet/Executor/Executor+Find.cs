using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand>
{
  public Task<TEntity?> ExecuteFindAsync<TEntity, TKey>(TDbCommand command, TKey key, in NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _commandBuilder.BuildFindCommand(command, key, in spec);
    _interceptor.OnCommandBuilt(OperationType.Find, command);

    return ExecuteFindAsync(command, spec, cancellationToken);
  }

  private async Task<TEntity?> ExecuteFindAsync<TEntity, TKey>(TDbCommand command, NavigationSpec<TEntity, TKey> spec, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

    return await _materializer.ReadEntityAsync(reader, spec, cancellationToken);
  }
}
