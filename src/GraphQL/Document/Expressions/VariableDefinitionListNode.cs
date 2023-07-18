using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class VariableDefinitionListNode : ListIterator<IVariable, IVariableDefinitionNode>, IVariableDefinitionListNode,
  IEnumerable<IVariableDefinitionNode>, IEnumerator<IVariableDefinitionNode>
{
  private readonly IReadOnlyList<IVariable> _variables;

  public VariableDefinitionListNode(IReadOnlyList<IVariable> variables)
  {
    _variables = variables;
  }

  public IEnumerable<IVariableDefinitionNode> VariableDefinitions => this;

  protected override IReadOnlyList<IVariable> List => _variables;

  protected override IVariableDefinitionNode OnCurrent(IVariable element)
  {
    return new VariableDefinitionNode(element);
  }
}
