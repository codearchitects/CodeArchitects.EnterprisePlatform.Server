using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IVariableNode
{
  ReadOnlySpan<char> IVariableNode.Name
  {
    get
    {
      Expect(TokenKind.Dollar);
      _lexer.MoveNext();

      Expect(TokenKind.Name);
      var value = _lexer.Value;
      _lexer.MoveNext();

      return value;
    }
  }
}
