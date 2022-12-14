using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor
{
  public async Task ExecuteDeleteCommandAsync<TEntity, TKey>(DbCommand command, TKey key, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _commandBuilder.BuildDeleteCommand(command, key, model);

    int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    if (affectedRows == 0)
      throw new DBConcurrencyException(); // TODO: Message
  }

  public Task ExecuteDeleteCommandAsync<TEntity, TKey>(DbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return ExecuteDeleteCommandAsync(command, entity, model, cancellationToken);
  }

  private async Task ExecuteDeleteCommandAsync(DbCommand command, object node, IEntityModel model, CancellationToken cancellationToken)
  {
    _commandBuilder.BuildDeleteCommand(command, node, model);

    int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    if (affectedRows == 0)
      throw new DBConcurrencyException(); // TODO: Message
  }
}
