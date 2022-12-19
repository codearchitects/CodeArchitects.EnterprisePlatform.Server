using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Materialization;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Tracking;
using System.Collections;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand> : IExecutor<TDbCommand>
  where TDbCommand : DbCommand
{
  private readonly ICommandBuilder<TDbCommand> _commandBuilder;
  private readonly IMaterializer _materializer;
  private readonly ICommandInterceptor<TDbCommand> _interceptor;
  private readonly ITrackingContext _trackingContext;

  public Executor(
    ICommandBuilder<TDbCommand> commandBuilder,
    IMaterializer materializer,
    ICommandInterceptorAggregator<TDbCommand> interceptor,
    ITrackingContext trackingContext)
  {
    _commandBuilder = commandBuilder;
    _materializer = materializer;
    _interceptor = interceptor;
    _trackingContext = trackingContext;
  }

  public void VisitGraph<TState>(object node, IEntityModel model, TState state, VisitNodeCallback<TState> callback)
  {
    foreach (INavigationModel navigationModel in model.Navigations)
    {
      if (navigationModel is not IAccessibleNavigationModel accessibleNavigationModel)
        continue;

      object? child = accessibleNavigationModel.GetValue(node);
      if (child is null)
        continue;

      if (accessibleNavigationModel.IsCollection)
      {
        IEnumerable collection = CopyCollection(accessibleNavigationModel.CollectionAccessor, node, child);

        foreach (object? element in collection)
        {
          if (element is null)
            continue;

          bool continueVisiting = callback(element, accessibleNavigationModel.To, new NavigationContext(node, accessibleNavigationModel), state);
          if (!continueVisiting)
            continue;

          VisitGraph(element, accessibleNavigationModel.To, state, callback);
        }
      }
      else
      {
        bool continueVisiting = callback(child, accessibleNavigationModel.To, new NavigationContext(node, accessibleNavigationModel), state);
        if (!continueVisiting)
          continue;

        VisitGraph(child, accessibleNavigationModel.To, state, callback);
      }
    }
  }

  public async Task VisitGraphAsync<TState>(object node, IEntityModel model, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
  {
    foreach (INavigationModel navigationModel in model.Navigations)
    {
      if (navigationModel is not IAccessibleNavigationModel accessibleNavigationModel)
        continue;

      object? child = accessibleNavigationModel.GetValue(node);
      if (child is null)
        continue;

      if (accessibleNavigationModel.IsCollection)
      {
        IEnumerable collection = CopyCollection(accessibleNavigationModel.CollectionAccessor, node, child);

        foreach (object? element in collection)
        {
          if (element is null)
            continue;

          bool continueVisiting = await callback(element, accessibleNavigationModel.To, new NavigationContext(node, accessibleNavigationModel), state, cancellationToken);
          if (!continueVisiting)
            continue;

          await VisitGraphAsync(element, accessibleNavigationModel.To, state, callback, cancellationToken);
        }
      }
      else
      {
        bool continueVisiting = await callback(child, accessibleNavigationModel.To, new NavigationContext(node, accessibleNavigationModel), state, cancellationToken);
        if (!continueVisiting)
          continue;

        await VisitGraphAsync(child, accessibleNavigationModel.To, state, callback, cancellationToken);
      }
    }
  }

  private static IEnumerable CopyCollection(ICollectionAccessor accessor, object node, object child)
  {
    _ = accessor.TryGetNonEnumeratedCount(node, out int count);
    List<object?> list = new(count);
    foreach (object? element in (IEnumerable)child)
    {
      list.Add(element);
    }

    return list;
  }

  private readonly record struct VisitGraphState(Executor<TDbCommand> Self, TDbCommand Command);
}
