using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;
using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : ISelectionSetNode, IEnumerable<ISelectionNode>, IEnumerator<ISelectionNode>
{
  IEnumerable<ISelectionNode> ISelectionSetNode.Selections => this;

  ISelectionNode IEnumerator<ISelectionNode>.Current => this;

  IEnumerator<ISelectionNode> IEnumerable<ISelectionNode>.GetEnumerator()
  {
    Expect(TokenKind.LeftBrace);

    _lexer.MoveNext();

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
