using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IArgumentListNode, IEnumerable<IArgumentNode>, IEnumerator<IArgumentNode>
{
  IEnumerable<IArgumentNode> IArgumentListNode.Arguments => this;

  IArgumentNode IEnumerator<IArgumentNode>.Current => this;

  IEnumerator<IArgumentNode> IEnumerable<IArgumentNode>.GetEnumerator()
  {
    Debug.Assert(_lexer.TokenKind is TokenKind.LeftParenthesis);
    
    _lexer.MoveNext();

    if (_lexer.TokenKind is TokenKind.RightParenthesis)
      throw LiteralGraphDocument.Unexpected(in _lexer); // Validate against a "()" syntax

    SetIterator(IteratorKind.Argument);
    return this;
  }

  private bool ArgumentMoveNext()
  {
    if (_lexer.TokenKind is not TokenKind.RightParenthesis)
      return true;

    _lexer.MoveNext();
    return false;
  }
}
