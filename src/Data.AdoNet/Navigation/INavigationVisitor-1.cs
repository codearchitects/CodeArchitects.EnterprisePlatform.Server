namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationVisitor<TResult>
{
  TResult VisitSimpleNode(INavigationSimpleNode navigation);
  
  TResult VisitSimpleLeaf(INavigationSimpleLeaf navigation);
  
  TResult VisitSkipNode(INavigationSkipNode navigation);
  
  TResult VisitSkipLeaf(INavigationSkipLeaf navigation);
}
