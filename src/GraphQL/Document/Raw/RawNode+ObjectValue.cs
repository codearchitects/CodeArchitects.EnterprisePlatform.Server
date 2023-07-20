using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IObjectValueNode, IEnumerable<IObjectFieldNode>, IEnumerator<IObjectFieldNode>
{
  IEnumerable<IObjectFieldNode> IObjectValueNode.Fields => this;

  IObjectFieldNode IEnumerator<IObjectFieldNode>.Current => this;

  IEnumerator<IObjectFieldNode> IEnumerable<IObjectFieldNode>.GetEnumerator()
  {
    Expect(TokenKind.LeftBrace);

    _lexer.MoveNext();

    SetIterator(IteratorKind.ObjectValue);
    return this;
  }

  private bool ObjectFieldMoveNext()
  {
    if (_lexer.TokenKind is not TokenKind.RightBrace)
      return true;

    _lexer.MoveNext();
    return false;
  }
}
