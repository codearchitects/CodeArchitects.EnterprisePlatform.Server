using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Tracking;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor
{
  public async Task ExecuteUpdateCommandAsync<TEntity, TKey>(DbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    AsyncVisitNodeCallback<VisitGraphState> callback = static async delegate (object node, IEntityModel model, NavigationContext context, VisitGraphState state, CancellationToken cancellationToken)
    {
      (Executor self, DbCommand command) = state;
      (object parent, INavigationModel navigationModel) = context;

      TrackingState trackingState = self._trackingContext.GetTrackingState(node);

      switch (navigationModel)
      {
        case ISimpleNavigationModel simpleNavigationModel:
          if (simpleNavigationModel.IsOnDependent)
          {
            if (simpleNavigationModel.AssociationKind is AssociationKind.Aggregation)
              return false;

            switch (trackingState)
            {
              case TrackingState.Added:
                await self.ExecuteUpdateCommandAsync(command, node, simpleNavigationModel.Inverse.NavigationEntity!, in context, cancellationToken);
                break;
              case TrackingState.Detached:
                break;
              default:
                throw new InvalidTrackingStateException(model.Type.Name, trackingState);
            }

            return false;
          }

          if (simpleNavigationModel.AssociationKind is AssociationKind.Composition)
          {
            switch (trackingState)
            {
              case TrackingState.Added:
                await self.ExecuteUpdateCommandAsync(command, node, simpleNavigationModel.NavigationEntity, in context, cancellationToken);
                break;
              case TrackingState.Removed:
                await self.ExecuteUpdateCommandAsync(command, node, simpleNavigationModel.NavigationEntity, context.WithRemovedParent(), cancellationToken);
                if (simpleNavigationModel.IsCollection)
                {
                  // TODO: Remove from collection
                }
                else
                {
                  simpleNavigationModel.SetValue!(parent, null);
                }
                break;
              case TrackingState.Detached or TrackingState.Unchanged:
                break;
              default:
                throw new InvalidTrackingStateException(model.Type.Name, trackingState);
            }

            return false;
          }

          switch (trackingState)
          {
            case TrackingState.Added:
              await self.ExecuteInsertCommandAsync(command, node, model, in context, cancellationToken);
              break;
            case TrackingState.Removed:
              await self.ExecuteDeleteCommandAsync(command, node, model, cancellationToken);
              break;
            case TrackingState.Modified:
              await self.ExecuteUpdateCommandAsync(command, node, model, in context, cancellationToken);
              break;
          }

          return true;

        case ISkipNavigationModel skipNavigationModel:
          object joinEntity;

          switch (trackingState)
          {
            case TrackingState.Added:
              joinEntity = skipNavigationModel.CreateJoin(parent, node);
              await self.ExecuteInsertCommandAsync(command, joinEntity, skipNavigationModel.JoinEntity, default, cancellationToken);
              break;
            case TrackingState.Removed:
              joinEntity = skipNavigationModel.CreateJoin(parent, node);
              await self.ExecuteDeleteCommandAsync(command, joinEntity, skipNavigationModel.JoinEntity, cancellationToken);
              // TODO: Remove from collection
              break;
            case TrackingState.Modified:
              throw new InvalidTrackingStateException(model.Type.Name, TrackingState.Modified);
          }

          return false;

        default:
          Debug.Fail("This point should be unreacheable.");
          return false;
      }
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
