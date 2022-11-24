namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationVisitor<TState>
{
  void VisitNode(INavigationNode navigation, in TState state);
  void VisitLeaf(INavigationLeaf navigation, in TState state);
}
