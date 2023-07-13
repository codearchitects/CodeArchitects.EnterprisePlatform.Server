using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

internal interface IOperationDefinitionNode // TODO: The AST should be as comprehensive and structured (conform to the original hierarchy) as possible and nodes should validate their values (e.g., names should conform to [_A-Za-z][_0-9A-Za-z]*)
{
  OperationType OperationType { get; }
  
  string? Name { get; }

  IEnumerable<IVariable> Variables { get; }

  IEnumerable<IDirectiveNode> Directives { get; }

  ISelectionSetNode SelectionSet { get; }
}
