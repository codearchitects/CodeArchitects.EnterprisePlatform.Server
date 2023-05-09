using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

/// <summary>
/// Represents an algorithm that is executed on a tree graph.
/// </summary>
[Experimental]
public interface IGraphVisitor
{
  /// <summary>
  /// Executes the algorithm on the root of the tree.
  /// </summary>
  /// <param name="root">The root entity.</param>
  /// <param name="model">The entity's model.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  bool VisitRoot(object root, IEntityModel model);

  /// <summary>
  /// Executes the algorithm on a simple (one-to-many) navigation collection.
  /// </summary>
  /// <param name="nodes">The collection of entities.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  bool VisitSimpleCollection(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSimpleNavigationModel> context);

  /// <summary>
  /// Executes the algorithm on a skip (many-to-many) navigation collection.
  /// </summary>
  /// <param name="nodes">The collection of entities.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  bool VisitSkipCollection(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSkipNavigationModel> context);

  /// <summary>
  /// Executes the algorithm on a reference (one-to-one) navigation node.
  /// </summary>
  /// <param name="node">The entity.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  bool VisitReferenceNode(object node, NavigationContext<IAccessibleSimpleNavigationModel> context);

  /// <summary>
  /// Executes the algorithm on a simple (one-to-many) navigation collection node.
  /// </summary>
  /// <param name="node">The entity.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  bool VisitSimpleCollectionNode(object node, NavigationContext<IAccessibleSimpleNavigationModel> context);

  /// <summary>
  /// Executes the algorithm on a skip (many-to-many) navigation collection node.
  /// </summary>
  /// <param name="node">The entity.</param>
  /// <param name="context">The <see cref="NavigationContext"/> for this navigation.</param>
  /// <returns><see langword="true"/> if the visitor should continue visiting the graph, <see langword="false"/> otherwise.</returns>
  bool VisitSkipCollectionNode(object node, NavigationContext<IAccessibleSkipNavigationModel> context);
}
