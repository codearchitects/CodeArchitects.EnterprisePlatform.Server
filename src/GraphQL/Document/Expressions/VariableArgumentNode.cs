using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class VariableArgumentNode : IArgumentNode, IVariableNode
{
  private readonly IVariable _variable;
  private readonly string? _name;

  public VariableArgumentNode(IVariable variable, string? name)
  {
    _variable = variable;
    _name = name;
  }

  public ReadOnlySpan<char> Name => _name is null ? _variable.Name : _name.AsSpan();

  public object? Value => this;

  ReadOnlySpan<char> IVariableNode.Name => _variable.Name;
}
