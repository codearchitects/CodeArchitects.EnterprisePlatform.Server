namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IVariableDefinitionListNode
{
  IEnumerable<IVariableDefinitionNode> VariableDefinitions { get; }
}
