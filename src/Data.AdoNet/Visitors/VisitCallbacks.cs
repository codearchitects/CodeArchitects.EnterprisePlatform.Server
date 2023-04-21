using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

/// <summary>
/// Represents a callback used to visit a node in a graph during a graph traversal.
/// </summary>
/// <typeparam name="TState">The type of the state object passed to the callback.</typeparam>
/// <param name="node">The node being visited.</param>
/// <param name="model">The entity model used to access the properties of the node.</param>
/// <param name="context">The navigation context used to access the graph being traversed.</param>
/// <param name="state">The state object passed to the callback.</param>
/// <returns>A value indicating whether the traversal should continue.</returns>
[Experimental]
public delegate bool VisitNodeCallback<TState>(object node, IEntityModel model, NavigationContext context, TState state);

/// <summary>
/// Represents an asynchronous callback used to visit a node in a graph during a graph traversal.
/// </summary>
/// <typeparam name="TState">The type of the state object passed to the callback.</typeparam>
/// <param name="node">The node being visited.</param>
/// <param name="model">The entity model used to access the properties of the node.</param>
/// <param name="context">The navigation context used to access the graph being traversed.</param>
/// <param name="state">The state object passed to the callback.</param>
/// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
/// <returns>A value indicating whether the traversal should continue.</returns>
[Experimental]
public delegate Task<bool> AsyncVisitNodeCallback<TState>(object node, IEntityModel model, NavigationContext context, TState state, CancellationToken cancellationToken);
