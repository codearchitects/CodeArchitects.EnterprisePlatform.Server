using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : ISelectionSetNode, IEnumerable<ISelectionNode>, IEnumerator<ISelectionNode>
{
  IEnumerable<ISelectionNode> ISelectionSetNode.Selections
  {
    get
    {
      Expect(TokenKind.LeftBrace);
      _lexer.MoveNext();

      return this;
    }
  }

  ISelectionNode IEnumerator<ISelectionNode>.Current => this;

  IEnumerator<ISelectionNode> IEnumerable<ISelectionNode>.GetEnumerator()
  {
    if (_lexer.TokenKind is TokenKind.RightBrace)
      throw Unexpected(); // Validate against a "{}" syntax

    SetIterator(IteratorKind.Selection);
    return this;
  }

  private bool SelectionMoveNext()
  {
    if (_lexer.TokenKind is not TokenKind.RightBrace)
      return true;

    _lexer.MoveNext();
    return false;
  }
}
