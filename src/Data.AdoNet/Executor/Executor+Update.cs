using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using CodeArchitects.Platform.Data.Features.Associations;
using CodeArchitects.Platform.Data.Tracking;
using System.Data;
using System.Diagnostics;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand>
{
  public Task ExecuteUpdateAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    ExecuteUpdateGraphVisitor visitor = new(this, command);
    return Graph.VisitAsync(entity, model, visitor, cancellationToken);
  }

  private async Task ExecuteUpdateAsync(TDbCommand command, object node, IEntityModel model, NavigationContext context, CancellationToken cancellationToken)
  {
    _commandBuilder.BuildUpdateCommand(command, node, model, context);
    _interceptor.OnCommandBuilt(OperationType.Update, command);

    int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    if (affectedRows == 0)
      throw new DBConcurrencyException(); // TODO: Message
  }

  private class ExecuteUpdateGraphVisitor : IAsyncGraphVisitor
  {
    private readonly Executor<TDbCommand> _executor;
    private readonly TDbCommand _command;

    public ExecuteUpdateGraphVisitor(Executor<TDbCommand> executor, TDbCommand command)
    {
      _executor = executor;
      _command = command;
    }

    public virtual async Task<bool> VisitRootAsync(object root, IEntityModel model, CancellationToken cancellationToken = default)
    {
      await _executor.ExecuteUpdateAsync(_command, root, model, default, cancellationToken);
      return true;
    }

    public Task<bool> VisitSimpleCollectionAsync(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default)
    {
      IAccessibleSimpleNavigationModel navigationModel = context.NavigationModel;
      return Task.FromResult(!navigationModel.IsOnDependent || navigationModel.AssociationKind is not AssociationKind.IntraAggregate);
    }

    public Task<bool> VisitSkipCollectionAsync(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSkipNavigationModel> context, CancellationToken cancellationToken = default)
    {
      return Task.FromResult(true);
    }

    public Task<bool> VisitSimpleCollectionNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default)
    {
      return VisitSimpleNodeAsync(node, context, cancellationToken);
    }

    public async Task<bool> VisitSkipCollectionNodeAsync(object node, NavigationContext<IAccessibleSkipNavigationModel> context, CancellationToken cancellationToken = default)
    {
      IAccessibleSkipNavigationModel navigationModel = context.NavigationModel;
      TrackingState trackingState = _executor._trackingContext.GetTrackingState(node);
      object junctionEntity;

      switch (trackingState)
      {
        case TrackingState.Added:
          junctionEntity = navigationModel.CreateJunction(context.Parent, node);
          await _executor.ExecuteInsertAsync(_command, junctionEntity, navigationModel.JunctionEntity, default, cancellationToken);
          break;
        case TrackingState.Removed:
          junctionEntity = navigationModel.CreateJunction(context.Parent, node);
          await _executor.ExecuteRemoveAsync(_command, junctionEntity, navigationModel.JunctionEntity, cancellationToken);
          navigationModel.CollectionAccessor.Remove(context.Parent, node);
          break;
        case TrackingState.Modified:
          throw new InvalidTrackingStateException(context.EntityModel.Type.Name, TrackingState.Modified);
      }

      return false;
    }

    public Task<bool> VisitReferenceNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default)
    {
      IAccessibleSimpleNavigationModel navigationModel = context.NavigationModel;
      if (navigationModel.IsOnDependent && navigationModel.AssociationKind is AssociationKind.IntraAggregate)
        return Task.FromResult(false);

      return VisitSimpleNodeAsync(node, context, cancellationToken);
    }

    private async Task<bool> VisitSimpleNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default)
    {
      IAccessibleSimpleNavigationModel navigationModel = context.NavigationModel;
      TrackingState trackingState = _executor._trackingContext.GetTrackingState(node);

      if (navigationModel.IsOnDependent)
      {
        Debug.Assert(navigationModel.AssociationKind is AssociationKind.InterAggregate, "Did not expect an intra-aggregate on dependent association.");

        switch (trackingState)
        {
          case TrackingState.Added:
            await _executor.ExecuteUpdateAsync(_command, node, navigationModel.Inverse.NavigationEntity!, context, cancellationToken);
            break;
          case TrackingState.Detached:
            break;
          default:
            throw new InvalidTrackingStateException(context.EntityModel.Type.Name, trackingState);
        }

        return false;
      }

      if (navigationModel.AssociationKind is AssociationKind.InterAggregate)
      {
        switch (trackingState)
        {
          case TrackingState.Added:
            await _executor.ExecuteUpdateAsync(_command, node, navigationModel.NavigationEntity, context, cancellationToken);
            break;
          case TrackingState.Removed:
            await _executor.ExecuteUpdateAsync(_command, node, navigationModel.NavigationEntity, context.WithRemovedParent(), cancellationToken);
            
            if (navigationModel.IsCollection)
            {
              navigationModel.CollectionAccessor.Remove(context.Parent, node);
            }
            else
            {
              navigationModel.SetValue!(context.Parent, null);
            }
            break;
          case TrackingState.Detached or TrackingState.Unchanged:
            break;
          default:
            throw new InvalidTrackingStateException(context.EntityModel.Type.Name, trackingState);
        }

        return false;
      }

      switch (trackingState)
      {
        case TrackingState.Added:
          await _executor.ExecuteInsertAsync(_command, node, context.EntityModel, context, cancellationToken);
          break;
        case TrackingState.Removed:
          await _executor.ExecuteRemoveAsync(_command, node, context.EntityModel, cancellationToken);
          break;
        case TrackingState.Modified:
          await _executor.ExecuteUpdateAsync(_command, node, context.EntityModel, context, cancellationToken);
          break;
      }

      return true;
    }
  }
}
