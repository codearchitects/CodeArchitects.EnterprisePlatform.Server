using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IOperationDefinitionNode
{
  OperationType IOperationDefinitionNode.OperationType
  {
    get
    {
      return LiteralGraphDocument.GetOperationType(ref _lexer);
    }
  }

  ReadOnlySpan<char> IOperationDefinitionNode.Name
  {
    get
    {
      return LiteralGraphDocument.GetName(ref _lexer);
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
