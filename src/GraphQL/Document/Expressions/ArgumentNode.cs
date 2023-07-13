using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ArgumentNode : IArgumentNode
{
  public ArgumentNode(string name, object? value)
  {
    Name = name;
    Value = value;
  }

  public string Name { get; }

  public object? Value { get; }
}
