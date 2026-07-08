namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IInlineFragmentNode : ISelectionNode
{
  ITypeConditionNode? TypeCondition { get; }

  IDirectiveListNode? DirectiveList { get; }

  ISelectionSetNode SelectionSet { get; }
}
