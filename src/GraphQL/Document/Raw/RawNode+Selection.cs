using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : ISelectionNode
{
  SelectionNodeKind ISelectionNode.SelectionKind
  {
    get
    {
      if (_lexer.TokenKind is TokenKind.Name)
        return SelectionNodeKind.Field;

      Expect(TokenKind.Spread);
      _lexer.MoveNext();

      return _lexer.TokenKind is TokenKind.Name && _lexer.ValueSpan is not "on"
        ? SelectionNodeKind.FragmentSpread
        : SelectionNodeKind.InlineFragment;
    }
  }
}
