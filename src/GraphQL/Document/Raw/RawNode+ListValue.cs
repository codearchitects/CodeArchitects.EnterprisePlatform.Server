using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IListValueNode, IEnumerable<IValueNode>, IEnumerator<IValueNode>
{
  IEnumerable<IValueNode> IListValueNode.Values => this;

  IValueNode IEnumerator<IValueNode>.Current => this;

  IEnumerator<IValueNode> IEnumerable<IValueNode>.GetEnumerator()
  {
    Expect(TokenKind.LeftBracket);

    _lexer.MoveNext();

    SetIterator(IteratorKind.ListValue);
    return this;
  }

  private bool ListValueMoveNext()
  {
    if (_lexer.TokenKind is not TokenKind.RightBracket)
      return true;

    _lexer.MoveNext();
    return false;
  }
}
