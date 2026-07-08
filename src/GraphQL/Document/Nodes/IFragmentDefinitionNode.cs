namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IFragmentDefinitionNode : IExecutableDefinitionNode
{
  ReadOnlySpan<char> FragmentName { get; }
  
  ITypeConditionNode TypeCondition { get; }
  
  IDirectiveListNode? DirectiveList { get; }
  
  ISelectionSetNode SelectionSet { get; }
}
