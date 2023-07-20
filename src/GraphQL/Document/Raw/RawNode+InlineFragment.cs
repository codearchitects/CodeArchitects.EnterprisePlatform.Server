using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IInlineFragmentNode
{
  ITypeConditionNode? IInlineFragmentNode.TypeCondition
  {
    get
    {
      if (_lexer.TokenKind is TokenKind.Name && _lexer.Value is "on")
      {
        _lexer.MoveNext();
        return this;
      }

      return null;
    }
  }

  IDirectiveListNode? IInlineFragmentNode.DirectiveList
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.At)
        return null;

      return this;
    }
  }

  ISelectionSetNode IInlineFragmentNode.SelectionSet
  {
    get
    {
      Expect(TokenKind.LeftBrace);
      return this;
    }
  }
}
