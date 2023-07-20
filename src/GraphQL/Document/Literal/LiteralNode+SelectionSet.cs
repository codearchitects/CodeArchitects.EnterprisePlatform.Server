using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : ISelectionSetNode, IEnumerable<ISelectionNode>, IEnumerator<ISelectionNode>
{
  IEnumerable<ISelectionNode> ISelectionSetNode.Selections => this;

  ISelectionNode IEnumerator<ISelectionNode>.Current => this;

  IEnumerator<ISelectionNode> IEnumerable<ISelectionNode>.GetEnumerator()
  {
    Debug.Assert(_lexer.TokenKind is TokenKind.LeftBrace);

    _lexer.MoveNext();

    if (_lexer.TokenKind is TokenKind.RightBrace)
      throw LiteralGraphDocument.Unexpected(in _lexer); // Validate against a "{}" syntax

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
