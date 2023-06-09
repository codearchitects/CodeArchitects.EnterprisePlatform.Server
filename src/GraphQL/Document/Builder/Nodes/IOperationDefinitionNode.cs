using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

internal interface IOperationDefinitionNode
{
  OperationType OperationType { get; }
  
  string? Name { get; }

  IEnumerable<IVariable> Variables { get; }

  IEnumerable<IDirectiveNode> Directives { get; }

  ISelectionSetNode SelectionSet { get; }
}
