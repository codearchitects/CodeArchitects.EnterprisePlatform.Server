namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationVisitor<TResult>
{
  TResult VisitNode(INavigationNode navigation);
  TResult VisitLeaf(INavigationLeaf navigation);
  TResult VisitSkipNode(INavigationSkipNode navigation);
  TResult VisitSkipLeaf(INavigationSkipLeaf navigation);
}
