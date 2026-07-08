namespace CodeArchitects.Platform.GraphQL.Document.Nodes;

public interface IFieldNode : ISelectionNode
{
  ReadOnlySpan<char> Alias { get; }

  ReadOnlySpan<char> FieldName { get; }

  IArgumentListNode? ArgumentList { get; }

  IDirectiveListNode? DirectiveList { get; }

  ISelectionSetNode? SelectionSet { get; }
}
