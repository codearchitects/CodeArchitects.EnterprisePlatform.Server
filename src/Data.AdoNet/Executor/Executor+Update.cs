using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using CodeArchitects.Platform.Data.Features.Associations;
using CodeArchitects.Platform.Data.Tracking;
using System.Diagnostics;
using System.Threading;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand>
{
  public void ExecuteUpdate<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    TryCreateConcurrencyToken(entity, model);
    ExecuteUpdateGraphVisitor visitor = new(this, command);
    Graph.Visit(entity, model, visitor);
  }

  public Task ExecuteUpdateAsync<TEntity, TKey>(TDbCommand command, TEntity entity, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    TryCreateConcurrencyToken(entity, model);
    ExecuteUpdateGraphVisitor visitor = new(this, command);
    return Graph.VisitAsync(entity, model, visitor, cancellationToken);
  }

  private void ExecuteUpdate(TDbCommand command, object node, IEntityModel model, NavigationContext context)
  {
    BuildUpdateCommand(command, node, model, context);
    int affectedRows = command.ExecuteNonQuery();
    CheckConcurrency(affectedRows);
  }

  private async Task ExecuteUpdateAsync(TDbCommand command, object node, IEntityModel model, NavigationContext context, CancellationToken cancellationToken)
  {
    BuildUpdateCommand(command, node, model, context);
    int affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
    CheckConcurrency(affectedRows);
  }

  private void BuildUpdateCommand(TDbCommand command, object node, IEntityModel model, NavigationContext context)
  {
    _commandBuilder.BuildUpdateCommand(command, node, model, context);
    _interceptor.OnCommandBuilt(OperationType.Update, command);
  }

  private class ExecuteUpdateGraphVisitor : IGraphVisitor, IAsyncGraphVisitor
  {
    private readonly Executor<TDbCommand> _executor;
    private readonly TDbCommand _command;

    public ExecuteUpdateGraphVisitor(Executor<TDbCommand> executor, TDbCommand command)
    {
      _executor = executor;
      _command = command;
    }

    public bool VisitRoot(object root, IEntityModel model)
    {
      _executor.ExecuteUpdate(_command, root, model, default);
      return true;
    }

    public virtual async Task<bool> VisitRootAsync(object root, IEntityModel model, CancellationToken cancellationToken = default)
    {
      await _executor.ExecuteUpdateAsync(_command, root, model, default, cancellationToken);
      return true;
    }

    public bool VisitSimpleCollection(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSimpleNavigationModel> context)
    {
      return VisitSimpleCollectionCore(context);
    }

    public Task<bool> VisitSimpleCollectionAsync(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default)
    {
      return Task.FromResult(VisitSimpleCollectionCore(context));
    }

    public bool VisitSkipCollection(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSkipNavigationModel> context)
    {
      return true;
    }

    public Task<bool> VisitSkipCollectionAsync(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSkipNavigationModel> context, CancellationToken cancellationToken = default)
    {
      return Task.FromResult(true);
    }

    public bool VisitSimpleCollectionNode(object node, NavigationContext<IAccessibleSimpleNavigationModel> context)
    {
      return VisitSimpleNode(node, context);
    }

    public Task<bool> VisitSimpleCollectionNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default)
    {
      return VisitSimpleNodeAsync(node, context, cancellationToken);
    }

    public bool VisitSkipCollectionNode(object node, NavigationContext<IAccessibleSkipNavigationModel> context)
    {
      IAccessibleSkipNavigationModel navigationModel = context.NavigationModel;
      TrackingState trackingState = _executor._trackingContext.GetTrackingState(node);
      object junctionEntity;

      switch (trackingState)
      {
        case TrackingState.Added:
          junctionEntity = navigationModel.CreateJunction(context.Parent, node);
          _executor.ExecuteInsert(_command, junctionEntity, navigationModel.JunctionEntity, default);
          break;
        case TrackingState.Removed:
          junctionEntity = navigationModel.CreateJunction(context.Parent, node);
          _executor.ExecuteRemove(_command, junctionEntity, navigationModel.JunctionEntity);
          navigationModel.CollectionAccessor.Remove(context.Parent, node);
          break;
        case TrackingState.Modified:
          throw new InvalidTrackingStateException(context.EntityModel.Type.Name, TrackingState.Modified);
      }

      return false;
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

    public bool VisitReferenceNode(object node, NavigationContext<IAccessibleSimpleNavigationModel> context)
    {
      IAccessibleSimpleNavigationModel navigationModel = context.NavigationModel;
      if (navigationModel.IsOnDependent && navigationModel.AssociationKind is AssociationKind.IntraAggregate)
        return false;

      return VisitSimpleNode(node, context);
    }

    public Task<bool> VisitReferenceNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default)
    {
      IAccessibleSimpleNavigationModel navigationModel = context.NavigationModel;
      if (navigationModel.IsOnDependent && navigationModel.AssociationKind is AssociationKind.IntraAggregate)
        return Task.FromResult(false);

      return VisitSimpleNodeAsync(node, context, cancellationToken);
    }

    private static bool VisitSimpleCollectionCore(NavigationContext<IAccessibleSimpleNavigationModel> context)
    {
      IAccessibleSimpleNavigationModel navigationModel = context.NavigationModel;
      return !navigationModel.IsOnDependent || navigationModel.AssociationKind is not AssociationKind.IntraAggregate;
    }

    private bool VisitSimpleNode(object node, NavigationContext<IAccessibleSimpleNavigationModel> context)
    {
      IAccessibleSimpleNavigationModel navigationModel = context.NavigationModel;
      TrackingState trackingState = _executor._trackingContext.GetTrackingState(node);

      if (navigationModel.IsOnDependent)
      {
        Debug.Assert(navigationModel.AssociationKind is AssociationKind.InterAggregate, "Did not expect an intra-aggregate on dependent association.");

        switch (trackingState)
        {
          case TrackingState.Added:
            _executor.ExecuteUpdate(_command, node, navigationModel.Inverse.NavigationEntity!, context);
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
            _executor.ExecuteUpdate(_command, node, navigationModel.NavigationEntity, context);
            break;
          case TrackingState.Removed:
            _executor.ExecuteUpdate(_command, node, navigationModel.NavigationEntity, context.WithRemovedParent());

            if (navigationModel.IsCollection)
            {
              navigationModel.CollectionAccessor.Remove(context.Parent, node);
            }
            else
            {
              navigationModel.SetValue(context.Parent, null);
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
          _executor.ExecuteInsert(_command, node, context.EntityModel, context);
          break;
        case TrackingState.Removed:
          _executor.ExecuteRemove(_command, node, context.EntityModel);
          break;
        case TrackingState.Modified:
          _executor.ExecuteUpdate(_command, node, context.EntityModel, context);
          break;
      }

      return true;
    }

    private async Task<bool> VisitSimpleNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken)
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
              navigationModel.SetValue(context.Parent, null);
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
