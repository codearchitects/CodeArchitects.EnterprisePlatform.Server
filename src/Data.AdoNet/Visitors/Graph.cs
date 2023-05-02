using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Collections;

namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

internal static class Graph
{
  public static void Visit(object root, IEntityModel model, IGraphVisitor visitor)
  {
    bool continueVisiting = visitor.VisitRoot(root, model);
    if (!continueVisiting)
      return;

    VisitLeaves(root, model, visitor);
  }

  public static void Visit<TState>(object root, IEntityModel model, TState state, VisitNodeCallback<TState> callback)
  {
    bool continueVisiting = callback(root, model, default, state);
    if (!continueVisiting)
      return;

    VisitLeaves(root, model, state, callback);
  }

  public static async Task VisitAsync(object root, IEntityModel model, IAsyncGraphVisitor visitor, CancellationToken cancellationToken)
  {
    bool continueVisiting = await visitor.VisitRootAsync(root, model, cancellationToken);
    if (!continueVisiting)
      return;

    await VisitLeavesAsync(root, model, visitor, cancellationToken);
  }

  public static async Task VisitAsync<TState>(object root, IEntityModel model, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
  {
    bool continueVisiting = await callback(root, model, default, state, cancellationToken);
    if (!continueVisiting)
      return;

    await VisitLeavesAsync(root, model, state, callback, cancellationToken);
  }

  private static void VisitLeaves(object node, IEntityModel model, IGraphVisitor visitor)
  {
    foreach (INavigationModel navigationModel in model.Navigations)
    {
      switch (navigationModel)
      {
        case IAccessibleSimpleNavigationModel simpleNavigationModel:
          VisitSimpleNavigation(visitor, simpleNavigationModel, node);
          break;

        case IAccessibleSkipNavigationModel skipNavigationModel:
          VisitSkipNavigation(visitor, skipNavigationModel, node);
          break;
      }
    }

    static void VisitSimpleNavigation(IGraphVisitor visitor, IAccessibleSimpleNavigationModel navigationModel, object node)
    {
      object? child = navigationModel.GetValue(node);
      if (child is null)
        return;

      NavigationContext<IAccessibleSimpleNavigationModel> context = new(node, navigationModel);

      if (navigationModel.IsCollection)
      {
        IReadOnlyCollection<object> collection = CopyCollection(navigationModel.CollectionAccessor, node, child);
        bool continueVisiting = visitor.VisitSimpleCollection(collection, context);
        if (!continueVisiting)
          return;

        foreach (object element in collection)
        {
          continueVisiting = visitor.VisitSimpleCollectionNode(element, context);
          if (!continueVisiting)
            continue;

          VisitLeaves(element, navigationModel.To, visitor);
        }
      }
      else
      {
        bool continueVisiting = visitor.VisitReferenceNode(child, context);
        if (!continueVisiting)
          return;

        VisitLeaves(child, navigationModel.To, visitor);
      }
    }

    static void VisitSkipNavigation(IGraphVisitor visitor, IAccessibleSkipNavigationModel navigationModel, object node)
    {
      object? child = navigationModel.GetValue(node);
      if (child is null)
        return;

      NavigationContext<IAccessibleSkipNavigationModel> context = new(node, navigationModel);

      IReadOnlyCollection<object> collection = CopyCollection(navigationModel.CollectionAccessor, node, child);
      bool continueVisiting = visitor.VisitSkipCollection(collection, context);
      if (!continueVisiting)
        return;

      foreach (object element in collection)
      {
        continueVisiting = visitor.VisitSkipCollectionNode(element, context);
        if (!continueVisiting)
          continue;

        VisitLeaves(element, navigationModel.To, visitor);
      }
    }
  }

  private static void VisitLeaves<TState>(object node, IEntityModel model, TState state, VisitNodeCallback<TState> callback)
  {
    foreach (INavigationModel navigationModel in model.Navigations)
    {
      if (navigationModel is not IAccessibleNavigationModel accessibleNavigationModel)
        continue;

      object? child = accessibleNavigationModel.GetValue(node);
      if (child is null)
        continue;

      NavigationContext context = new(node, accessibleNavigationModel);

      if (accessibleNavigationModel.IsCollection)
      {
        IReadOnlyCollection<object> collection = CopyCollection(accessibleNavigationModel.CollectionAccessor, node, child);

        foreach (object element in collection)
        {
          bool continueVisiting = callback(element, accessibleNavigationModel.To, context, state);
          if (!continueVisiting)
            continue;

          VisitLeaves(element, accessibleNavigationModel.To, state, callback);
        }
      }
      else
      {
        bool continueVisiting = callback(child, accessibleNavigationModel.To, context, state);
        if (!continueVisiting)
          continue;

        VisitLeaves(child, accessibleNavigationModel.To, state, callback);
      }
    }
  }

  private static async Task VisitLeavesAsync(object node, IEntityModel model, IAsyncGraphVisitor visitor, CancellationToken cancellationToken)
  {
    foreach (INavigationModel navigationModel in model.Navigations)
    {
      switch (navigationModel)
      {
        case IAccessibleSimpleNavigationModel simpleNavigationModel:
          await VisitSimpleNavigationAsync(visitor, simpleNavigationModel, node, cancellationToken);
          break;

        case IAccessibleSkipNavigationModel skipNavigationModel:
          await VisitSkipNavigationAsync(visitor, skipNavigationModel, node, cancellationToken);
          break;
      }
    }

    static async Task VisitSimpleNavigationAsync(IAsyncGraphVisitor visitor, IAccessibleSimpleNavigationModel navigationModel, object node, CancellationToken cancellationToken)
    {
      object? child = navigationModel.GetValue(node);
      if (child is null)
        return;

      NavigationContext<IAccessibleSimpleNavigationModel> context = new(node, navigationModel);

      if (navigationModel.IsCollection)
      {
        IReadOnlyCollection<object> collection = CopyCollection(navigationModel.CollectionAccessor, node, child);
        bool continueVisiting = await visitor.VisitSimpleCollectionAsync(collection, context, cancellationToken);
        if (!continueVisiting)
          return;

        foreach (object element in collection)
        {
          continueVisiting = await visitor.VisitSimpleCollectionNodeAsync(element, context, cancellationToken);
          if (!continueVisiting)
            continue;

          await VisitLeavesAsync(element, navigationModel.To, visitor, cancellationToken);
        }
      }
      else
      {
        bool continueVisiting = await visitor.VisitReferenceNodeAsync(child, context, cancellationToken);
        if (!continueVisiting)
          return;

        await VisitLeavesAsync(child, navigationModel.To, visitor, cancellationToken);
      }
    }

    static async Task VisitSkipNavigationAsync(IAsyncGraphVisitor visitor, IAccessibleSkipNavigationModel navigationModel, object node, CancellationToken cancellationToken)
    {
      object? child = navigationModel.GetValue(node);
      if (child is null)
        return;

      NavigationContext<IAccessibleSkipNavigationModel> context = new(node, navigationModel);

      IReadOnlyCollection<object> collection = CopyCollection(navigationModel.CollectionAccessor, node, child);
      bool continueVisiting = await visitor.VisitSkipCollectionAsync(collection, context, cancellationToken);
      if (!continueVisiting)
        return;

      foreach (object element in collection)
      {
        continueVisiting = await visitor.VisitSkipCollectionNodeAsync(element, context, cancellationToken);
        if (!continueVisiting)
          continue;

        await VisitLeavesAsync(element, navigationModel.To, visitor, cancellationToken);
      }
    }
  }

  private static async Task VisitLeavesAsync<TState>(object node, IEntityModel model, TState state, AsyncVisitNodeCallback<TState> callback, CancellationToken cancellationToken)
  {
    foreach (INavigationModel navigationModel in model.Navigations)
    {
      if (navigationModel is not IAccessibleNavigationModel accessibleNavigationModel)
        continue;

      object? child = accessibleNavigationModel.GetValue(node);
      if (child is null)
        continue;

      NavigationContext context = new(node, accessibleNavigationModel);

      if (accessibleNavigationModel.IsCollection)
      {
        IReadOnlyCollection<object> collection = CopyCollection(accessibleNavigationModel.CollectionAccessor, node, child);

        foreach (object element in collection)
        {
          bool continueVisiting = await callback(element, accessibleNavigationModel.To, context, state, cancellationToken);
          if (!continueVisiting)
            continue;

          await VisitLeavesAsync(element, accessibleNavigationModel.To, state, callback, cancellationToken);
        }
      }
      else
      {
        bool continueVisiting = await callback(child, accessibleNavigationModel.To, context, state, cancellationToken);
        if (!continueVisiting)
          continue;

        await VisitLeavesAsync(child, accessibleNavigationModel.To, state, callback, cancellationToken);
      }
    }
  }

  private static IReadOnlyCollection<object> CopyCollection(ICollectionAccessor accessor, object node, object child)
  {
    _ = accessor.TryGetNonEnumeratedCount(node, out int count);
    List<object> list = new(count);

    IEnumerable collection = (IEnumerable)child;
    foreach (object? element in collection)
    {
      if (element is null)
        continue;

      list.Add(element);
    }

    return list;
  }
}
