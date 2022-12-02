using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Tracking;
using System.Data;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor
{
  public async Task ExecuteUpdateCommandAsync<TEntity, TKey>(DbConnection connection, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    using DbCommand command = connection.CreateCommand();

    AsyncVisitNodeCallback<VisitGraphState> callback = static async delegate (object node, IEntityModel model, NavigationContext context, VisitGraphState state, CancellationToken cancellationToken)
    {
      (Executor self, DbCommand command) = state;
      (object parent, INavigationModel navigationModel) = context;

      if (!navigationModel.IsOnDependent && navigationModel.IsAggregation)
      {
        switch (self._trackingContext.GetTrackingState(node))
        {
          case TrackingState.Added:
            await self.ExecuteInsertCommandAsync(command, node, model, in context, cancellationToken);
            break;
          case TrackingState.Removed:
            throw new NotSupportedException();
          case TrackingState.Modified:
            await self.ExecuteUpdateCommandAsync(command, node, model, in context, cancellationToken);
            break;
        }
        return true;
      }

      if (navigationModel is ISkipNavigationModel skipNavigationModel)
      {
        switch (self._trackingContext.GetTrackingState(node))
        {
          case TrackingState.Added:
            object joinEntity = skipNavigationModel.CreateJoin(parent, node);
            await self.ExecuteInsertCommandAsync(command, joinEntity, skipNavigationModel.JoinEntity, default, cancellationToken);
            break;
          case TrackingState.Removed:
            throw new NotSupportedException();
          case TrackingState.Modified:
            throw new InvalidTrackingStateException(model.Type.Name, TrackingState.Modified);
        }
      }

      return false;
    };

    await ExecuteUpdateCommandAsync(command, entity, model, default, cancellationToken);
    await VisitGraphAsync(entity, model, new VisitGraphState(this, command), callback, cancellationToken);
  }

  private Task ExecuteUpdateCommandAsync(DbCommand command, object node, IEntityModel model, in NavigationContext context, CancellationToken cancellationToken)
  {
    _commandBuilder.BuildUpdateCommand(command, node, model, in context);

    return ExecuteAsync(command, cancellationToken);

    static async Task ExecuteAsync(DbCommand command, CancellationToken cancellationToken)
    {
      int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
      if (affectedRows == 0)
        throw new DBConcurrencyException(); // TODO: Message
    }
  }
}
