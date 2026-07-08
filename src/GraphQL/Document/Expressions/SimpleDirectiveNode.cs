using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class SimpleDirectiveNode : IDirectiveNode
{
  private readonly string _name;

  public SimpleDirectiveNode(string name)
  {
    _name = name;
  }

  public ReadOnlySpan<char> Name => _name;

  public IArgumentListNode? ArgumentList => null;
}
