using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Features.Associations;
using CodeArchitects.Platform.Data.Tracking;
using System.Data;
using System.Diagnostics;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand>
{
  public async Task ExecuteUpdateAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    AsyncVisitNodeCallback<VisitGraphState> callback = static async delegate (object node, IEntityModel model, NavigationContext context, VisitGraphState state, CancellationToken cancellationToken)
    {
      (Executor<TDbCommand> self, TDbCommand command) = state;
      (object parent, INavigationModel navigationModel) = context;

      TrackingState trackingState = self._trackingContext.GetTrackingState(node);

      switch (navigationModel)
      {
        case IAccessibleSimpleNavigationModel simpleNavigationModel:
          if (simpleNavigationModel.IsOnDependent)
          {
            if (simpleNavigationModel.AssociationKind is AssociationKind.IntraAggregate)
              return false;

            switch (trackingState)
            {
              case TrackingState.Added:
                await self.ExecuteUpdateAsync(command, node, simpleNavigationModel.Inverse.NavigationEntity!, in context, cancellationToken);
                break;
              case TrackingState.Detached:
                break;
              default:
                throw new InvalidTrackingStateException(model.Type.Name, trackingState);
            }

            return false;
          }

          if (simpleNavigationModel.AssociationKind is AssociationKind.InterAggregate)
          {
            switch (trackingState)
            {
              case TrackingState.Added:
                await self.ExecuteUpdateAsync(command, node, simpleNavigationModel.NavigationEntity, in context, cancellationToken);
                break;
              case TrackingState.Removed:
                await self.ExecuteUpdateAsync(command, node, simpleNavigationModel.NavigationEntity, context.WithRemovedParent(), cancellationToken);
                if (simpleNavigationModel.IsCollection)
                {
                  simpleNavigationModel.CollectionAccessor.Remove(parent, node);
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
              await self.ExecuteInsertAsync(command, node, model, in context, cancellationToken);
              break;
            case TrackingState.Removed:
              await self.ExecuteRemoveAsync(command, node, model, cancellationToken);
              break;
            case TrackingState.Modified:
              await self.ExecuteUpdateAsync(command, node, model, in context, cancellationToken);
              break;
          }

          return true;

        case IAccessibleSkipNavigationModel skipNavigationModel:
          object junctionEntity;

          switch (trackingState)
          {
            case TrackingState.Added:
              junctionEntity = skipNavigationModel.CreateJunction(parent, node);
              await self.ExecuteInsertAsync(command, junctionEntity, skipNavigationModel.JunctionEntity, default, cancellationToken);
              break;
            case TrackingState.Removed:
              junctionEntity = skipNavigationModel.CreateJunction(parent, node);
              await self.ExecuteRemoveAsync(command, junctionEntity, skipNavigationModel.JunctionEntity, cancellationToken);
              skipNavigationModel.CollectionAccessor.Remove(parent, node);
              break;
            case TrackingState.Modified:
              throw new InvalidTrackingStateException(model.Type.Name, TrackingState.Modified);
          }

          return false;

        default:
          Debug.Fail("This point should be unreachable.");
          return false;
      }
    };

    await ExecuteUpdateAsync(command, entity, model, default, cancellationToken);
    await VisitGraphAsync(entity, model, new VisitGraphState(this, command), callback, cancellationToken);
  }

  private Task ExecuteUpdateAsync(TDbCommand command, object node, IEntityModel model, in NavigationContext context, CancellationToken cancellationToken)
  {
    _commandBuilder.BuildUpdateCommand(command, node, model, in context);
    _interceptor.OnCommandBuilt(OperationType.Update, command);

    return ExecuteAsync(command, cancellationToken);

    static async Task ExecuteAsync(TDbCommand command, CancellationToken cancellationToken)
    {
      int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
      if (affectedRows == 0)
        throw new DBConcurrencyException(); // TODO: Message
    }
  }
}
