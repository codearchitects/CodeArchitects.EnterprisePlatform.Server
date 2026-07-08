namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IOperationDefinitionNode : IExecutableDefinitionNode
{
  bool IsQueryShortHand { get; }

  OperationType OperationType { get; }

  ReadOnlySpan<char> Name { get; }

  IVariableDefinitionListNode? VariableDefinitionList { get; }

  IDirectiveListNode? DirectiveList { get; }

  ISelectionSetNode SelectionSet { get; }
}
