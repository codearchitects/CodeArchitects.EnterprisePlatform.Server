using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand>
{
  public async Task ExecuteRemoveAsync<TEntity, TKey>(TDbCommand command, TKey key, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _commandBuilder.BuildRemoveCommand(command, key, model);
    _interceptor.OnCommandBuilt(OperationType.Remove, command);

    int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    if (affectedRows == 0)
      throw new DBConcurrencyException(); // TODO: Message
  }

  public Task ExecuteRemoveAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return ExecuteRemoveAsync(command, entity, model, cancellationToken);
  }

  private async Task ExecuteRemoveAsync(TDbCommand command, object node, IEntityModel model, CancellationToken cancellationToken)
  {
    _commandBuilder.BuildRemoveCommand(command, node, model);
    _interceptor.OnCommandBuilt(OperationType.Remove, command);

    int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    if (affectedRows == 0)
      throw new DBConcurrencyException(); // TODO: Message
  }
}
