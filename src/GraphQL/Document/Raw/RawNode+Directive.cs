using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IDirectiveNode
{
  ReadOnlySpan<char> IDirectiveNode.Name
  {
    get
    {
      Expect(TokenKind.Name);

      var value = _lexer.Value;
      _lexer.MoveNext();

      return value;
    }
  }

  IArgumentListNode? IDirectiveNode.ArgumentList
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.LeftParenthesis)
        return null;

      return this;
    }
  }
}
