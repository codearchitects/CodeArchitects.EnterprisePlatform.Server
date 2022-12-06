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

          bool continueTracking = callback(element, navigationModel.To, new NavigationContext(node, navigationModel), state);
          if (!continueTracking)
            continue;

          VisitGraph(element, navigationModel.To, state, callback);
        }
      }
      else
      {
        bool continueTracking = callback(child, navigationModel.To, new NavigationContext(node, navigationModel), state);
        if (!continueTracking)
          continue;

        VisitGraph(child, navigationModel.To, state, callback);
      }
    }
  }

  public async Task VisitGraphAsync<TState>(object node, IEntityModel model, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
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

          bool continueTracking = await callback(element, navigationModel.To, new NavigationContext(node, navigationModel), state, cancellationToken);
          if (!continueTracking)
            continue;

          await VisitGraphAsync(element, navigationModel.To, state, callback, cancellationToken);
        }
      }
      else
      {
        bool continueTracking = await callback(child, navigationModel.To, new NavigationContext(node, navigationModel), state, cancellationToken);
        if (!continueTracking)
          continue;

        await VisitGraphAsync(child, navigationModel.To, state, callback, cancellationToken);
      }
    }
  }

  private readonly record struct VisitGraphState(Executor Self, DbCommand Command);
}
