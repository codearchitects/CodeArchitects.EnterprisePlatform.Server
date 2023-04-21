using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

[Experimental]
public interface IAsyncGraphVisitor
{
  Task<bool> VisitRootAsync(object root, IEntityModel model, CancellationToken cancellationToken = default);

  Task<bool> VisitSimpleCollectionAsync(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default);

  Task<bool> VisitSkipCollectionAsync(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSkipNavigationModel> context, CancellationToken cancellationToken = default);

  Task<bool> VisitReferenceNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default);

  Task<bool> VisitSimpleCollectionNodeAsync(object node, NavigationContext<IAccessibleSimpleNavigationModel> context, CancellationToken cancellationToken = default);

  Task<bool> VisitSkipCollectionNodeAsync(object node, NavigationContext<IAccessibleSkipNavigationModel> context, CancellationToken cancellationToken = default);
}
