using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand>
{
  public void ExecuteRemove<TEntity, TKey>(TDbCommand command, TKey key, IEntityModel<TEntity, TKey> model)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    BuildRemoveCommand(command, key, model);
    int affectedRows = command.ExecuteNonQuery();
    CheckConcurrency(affectedRows);
  }

  public async Task ExecuteRemoveAsync<TEntity, TKey>(TDbCommand command, TKey key, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    BuildRemoveCommand(command, key, model);
    int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    CheckConcurrency(affectedRows);
  }

  public void ExecuteRemove<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    ExecuteRemove(command, entity as object, model);
  }

  public Task ExecuteRemoveAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return ExecuteRemoveAsync(command, entity as object, model, cancellationToken);
  }

  private void ExecuteRemove(TDbCommand command, object entity, IEntityModel model)
  {
    BuildRemoveCommand(command, entity, model);
    int affectedRows = command.ExecuteNonQuery();
    CheckConcurrency(affectedRows);
  }

  private async Task ExecuteRemoveAsync(TDbCommand command, object entity, IEntityModel model, CancellationToken cancellationToken)
  {
    BuildRemoveCommand(command, entity, model);
    int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    CheckConcurrency(affectedRows);
  }

  private void BuildRemoveCommand(TDbCommand command, object entity, IEntityModel model)
  {
    _commandBuilder.BuildRemoveCommand(command, entity, model);
    _interceptor.OnCommandBuilt(OperationType.Remove, command);
  }

  private void BuildRemoveCommand<TEntity, TKey>(TDbCommand command, TKey key, IEntityModel<TEntity, TKey> model)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    _commandBuilder.BuildRemoveCommand(command, key, model);
    _interceptor.OnCommandBuilt(OperationType.Remove, command);
  }
}
