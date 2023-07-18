namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IVariableDefinitionNode
{
  IVariableNode Variable { get; }

  ITypeNode Type { get; }

  // TODO: DefaultValue
}
