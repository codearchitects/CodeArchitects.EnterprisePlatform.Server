using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class SimpleDirectiveNode : IDirectiveNode
{
  public SimpleDirectiveNode(string name)
  {
    Name = name;
  }

  public string Name { get; }

  public IEnumerable<IArgumentNode> Arguments => Enumerable.Empty<IArgumentNode>();
}
