using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : ISelectionNode
{
  SelectionNodeKind ISelectionNode.SelectionKind
  {
    get
    {
      if (_lexer.TokenKind is TokenKind.Name)
        return SelectionNodeKind.Field;

      Expect(TokenKind.Spread);
      _lexer.MoveNext();

      return _lexer.TokenKind is TokenKind.Name && _lexer.Value is not "on"
        ? SelectionNodeKind.FragmentSpread
        : SelectionNodeKind.InlineFragment;
    }
  }
}
