using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IArgumentListNode, IEnumerable<IArgumentNode>, IEnumerator<IArgumentNode>
{
  IEnumerable<IArgumentNode> IArgumentListNode.Arguments => this;

  IArgumentNode IEnumerator<IArgumentNode>.Current => this;

  IEnumerator<IArgumentNode> IEnumerable<IArgumentNode>.GetEnumerator()
  {
    Expect(TokenKind.LeftParenthesis);
    
    _lexer.MoveNext();

    if (_lexer.TokenKind is TokenKind.RightParenthesis)
      throw Unexpected(); // Validate against a "()" syntax

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
