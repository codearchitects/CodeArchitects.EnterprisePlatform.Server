using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : INamedTypeNode
{
  ReadOnlySpan<char> INamedTypeNode.Name
  {
    get
    {
      Expect(TokenKind.Name);

      ReadOnlySpan<char> value = _lexer.ValueSpan;

      do
      {
        _lexer.MoveNext();
      }
      while (_lexer.TokenKind is TokenKind.Bang or TokenKind.RightBracket);

      return value;
    }
  }
}
