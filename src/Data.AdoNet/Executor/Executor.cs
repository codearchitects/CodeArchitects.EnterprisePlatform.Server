using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Materialization;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Tracking;
using System.Collections;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor : IExecutor
{
  private readonly ICommandBuilder _commandBuilder;
  private readonly IMaterializer _materializer;
  private readonly ITrackingContext _trackingContext;

  public Executor(ICommandBuilder commandBuilder, IMaterializer materializer, ITrackingContext trackingContext)
  {
    _commandBuilder = commandBuilder;
    _materializer = materializer;
    _trackingContext = trackingContext;
  }

  public void VisitGraph<TState>(object node, IEntityModel model, TState state, VisitNodeCallback<TState> callback)
  {
    foreach (INavigationModel navigationModel in model.Navigations)
    {
      if (!navigationModel.HasMember)
        continue;

      object? child = navigationModel.GetValue(node);
      if (child is null)
        continue;

      if (navigationModel.IsCollection)
      {
        IEnumerable collection = (IEnumerable)child;
        foreach (object? element in collection)
        {
          if (element is null)
            continue;

          bool continueVisiting = callback(element, navigationModel.To, new NavigationContext(node, navigationModel), state);
          if (!continueVisiting)
            continue;

          VisitGraph(element, navigationModel.To, state, callback);
        }
      }
      else
      {
        bool continueVisiting = callback(child, navigationModel.To, new NavigationContext(node, navigationModel), state);
        if (!continueVisiting)
          continue;

        VisitGraph(child, navigationModel.To, state, callback);
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
        _ = accessibleNavigationModel.CollectionAccessor.TryGetNonEnumeratedCount(node, out int count);
        List<object?> list = new(count);
        foreach (object? element in (IEnumerable)child)
        {
          list.Add(element);
        }

        foreach (object? element in list)
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

  private readonly record struct VisitGraphState(Executor Self, DbCommand Command);
}
