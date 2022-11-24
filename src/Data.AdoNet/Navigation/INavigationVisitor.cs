namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationVisitor
{
  void VisitNode(INavigationNode navigation);
  void VisitLeaf(INavigationLeaf navigation);
}
