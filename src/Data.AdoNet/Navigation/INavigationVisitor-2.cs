namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationVisitor<TResult, TState>
{
  TResult VisitNode(INavigationNode navigation, in TState state);
  TResult VisitLeaf(INavigationLeaf navigation, in TState state);
  TResult VisitSkipNode(INavigationSkipNode navigation, in TState state);
  TResult VisitSkipLeaf(INavigationSkipLeaf navigation, in TState state);
}
