using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

[Experimental]
public interface IGraphVisitor
{
  bool VisitRoot(object root, IEntityModel model);

  bool VisitSimpleCollection(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSimpleNavigationModel> context);

  bool VisitSkipCollection(IReadOnlyCollection<object> nodes, NavigationContext<IAccessibleSkipNavigationModel> context);

  bool VisitReferenceNode(object node, NavigationContext<IAccessibleSimpleNavigationModel> context);

  bool VisitSimpleCollectionNode(object node, NavigationContext<IAccessibleSimpleNavigationModel> context);

  bool VisitSkipCollectionNode(object node, NavigationContext<IAccessibleSkipNavigationModel> context);
}
