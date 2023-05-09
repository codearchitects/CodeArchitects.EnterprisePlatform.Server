using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

/// <summary>
/// Represents an algorithm that is executed asynchronously on a tree graph.
/// </summary>
[Experimental]
public interface IAsyncGraphVisitor
{
  /// <summary>
  /// Executes the algorithm on the root of the tree.
  /// </summary>
  /// <param name="root">The root entity.</param>
  /// <param name="model">The entity's model.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  Task<bool> VisitRootAsync(object root, IEntityModel model, CancellationToken cancellationToken = default);

  /// <summary>
  /// Executes the algorithm on a simple (one-to-many) navigation collection.
  /// </summary>
  /// <param name="nodes">The collection of entities.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  Task<bool> VisitSimpleCollectionAsync(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default);

  /// <summary>
  /// Executes the algorithm on a skip (many-to-many) navigation collection.
  /// </summary>
  /// <param name="nodes">The collection of entities.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  Task<bool> VisitSkipCollectionAsync(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSkipNavigationModel> context, CancellationToken cancellationToken = default);

  /// <summary>
  /// Executes the algorithm on a reference (one-to-one) navigation node.
  /// </summary>
  /// <param name="node">The entity.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  Task<bool> VisitReferenceNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default);

  /// <summary>
  /// Executes the algorithm on a simple (one-to-many) navigation collection node.
  /// </summary>
  /// <param name="node">The entity.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  Task<bool> VisitSimpleCollectionNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default);

  /// <summary>
  /// Executes the algorithm on a skip (many-to-many) navigation collection node.
  /// </summary>
  /// <param name="node">The entity.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  Task<bool> VisitSkipCollectionNodeAsync(object node, NavigationContext<IAccessibleSkipNavigationModel> context, CancellationToken cancellationToken = default);
}
