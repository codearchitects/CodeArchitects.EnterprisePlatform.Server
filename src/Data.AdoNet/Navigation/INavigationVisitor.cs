namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationVisitor<TResult>
{
  TResult VisitSimpleNode(ISimpleNavigationNode navigation);
  
  TResult VisitSimpleLeaf(ISimpleNavigationLeaf navigation);
  
  TResult VisitSkipNode(ISkipNavigationNode navigation);
  
  TResult VisitSkipLeaf(ISkipNavigationLeaf navigation);
}
