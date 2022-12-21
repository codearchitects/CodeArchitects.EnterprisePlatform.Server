using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class Seeder<TDbConnection, TDbCommand> : ISeeder
  where TDbConnection : DbConnection
  where TDbCommand : DbCommand
{
  private readonly IStateManager<TDbConnection> _stateManager;
  private readonly ICommandBuilder<TDbCommand> _commandBuilder;
  private readonly IDataModel _model;

  public Seeder(
    IStateManager<TDbConnection> stateManager,
    ICommandBuilder<TDbCommand> commandBuilder,
    IDataModel model)
  {
    _stateManager = stateManager;
    _commandBuilder = commandBuilder;
    _model = model;
  }

  public void Seed<TEntity>(IEnumerable<TEntity> entities)
    where TEntity : class
  {
    // TODO: Check if the entities exist first

    IEntityModel entityModel = _model.GetEntity(typeof(TEntity));

    _stateManager.AddExecution(async (connection, transaction, cancellationToken) =>
    {
      foreach (TEntity entity in entities)
      {
        using TDbCommand command = (TDbCommand)connection.CreateCommand();
        command.Transaction = transaction;
        _commandBuilder.BuildInsertCommand(command, entity, entityModel, default);
        await command.ExecuteNonQueryAsync(cancellationToken);
      }
    }, true);
  }
}
