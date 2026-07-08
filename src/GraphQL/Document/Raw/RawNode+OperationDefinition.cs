using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IOperationDefinitionNode
{
  bool IOperationDefinitionNode.IsQueryShortHand
  {
    get
    {
      return _lexer.TokenKind is TokenKind.LeftBrace;
    }
  }

  OperationType IOperationDefinitionNode.OperationType
  {
    get
    {
      return RawGraphDocument.GetOperationType(ref _lexer);
    }
  }

  ReadOnlySpan<char> IOperationDefinitionNode.Name
  {
    get
    {
      return RawGraphDocument.GetName(ref _lexer);
    }
  }

  IVariableDefinitionListNode? IOperationDefinitionNode.VariableDefinitionList
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.LeftParenthesis)
        return null;

      return this;
    }
  }

  IDirectiveListNode? IOperationDefinitionNode.DirectiveList
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.At)
        return null;

      return this;
    }
  }

  ISelectionSetNode IOperationDefinitionNode.SelectionSet
  {
    get
    {
      Expect(TokenKind.LeftBrace);
      return this;
    }
  }
}
