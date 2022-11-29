namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationVisitor<TResult, TState>
{
  TResult VisitSimpleNode(INavigationSimpleNode navigation, in TState state);
  TResult VisitSimpleLeaf(INavigationSimpleLeaf navigation, in TState state);
  TResult VisitSkipNode(INavigationSkipNode navigation, in TState state);
  TResult VisitSkipLeaf(INavigationSkipLeaf navigation, in TState state);
}
