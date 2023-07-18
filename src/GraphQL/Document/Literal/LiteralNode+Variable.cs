using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IVariableNode
{
  ReadOnlySpan<char> IVariableNode.Name
  {
    get
    {
      Expect(TokenKind.Name);

      var value = _lexer.Value;
      _lexer.MoveNext();

      return value;
    }
  }
}
