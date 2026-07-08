using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand>
{
  public TEntity? ExecuteFind<TEntity, TKey>(TDbCommand command, TKey key, INavigationRoot<TEntity, TKey> root)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _commandBuilder.BuildFindCommand(command, key, root);
    _interceptor.OnCommandBuilt(OperationType.Find, command);

    using DbDataReader reader = command.ExecuteReader();
    return _materializer.ReadEntity(reader, root);
  }

  public async Task<TEntity?> ExecuteFindAsync<TEntity, TKey>(TDbCommand command, TKey key, INavigationRoot<TEntity, TKey> root, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _commandBuilder.BuildFindCommand(command, key, root);
    _interceptor.OnCommandBuilt(OperationType.Find, command);

    await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
    return await _materializer.ReadEntityAsync(reader, root, cancellationToken);
  }
}
