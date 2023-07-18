using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : INamedTypeNode
{
  ReadOnlySpan<char> INamedTypeNode.Name
  {
    get
    {
      Expect(TokenKind.Name);

      ReadOnlySpan<char> value = _lexer.Value;

      do
      {
        _lexer.MoveNext();
      }
      while (_lexer.TokenKind is TokenKind.Bang or TokenKind.RightBracket);

      return value;
    }
  }
}
