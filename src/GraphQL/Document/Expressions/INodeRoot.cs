namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal interface INodeRoot
{
  INodeContext Context { get; }

  void ReportFragment(GraphFragment fragment);
}
