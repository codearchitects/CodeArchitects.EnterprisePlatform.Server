namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationVisitor<TResult, TState>
{
  TResult VisitSimpleNode(ISimpleNavigationNode navigation, in TState state);
  
  TResult VisitSimpleLeaf(ISimpleNavigationLeaf navigation, in TState state);
  
  TResult VisitSkipNode(ISkipNavigationNode navigation, in TState state);
  
  TResult VisitSkipLeaf(ISkipNavigationLeaf navigation, in TState state);
}
