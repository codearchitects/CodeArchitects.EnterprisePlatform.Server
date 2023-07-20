using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IStringValueNode
{
  string IStringValueNode.Value
  {
    get
    {
      Expect(TokenKind.String);

      var value = _lexer.Value;
      _lexer.MoveNext();

      return value[1..^1].ToString();
    }
  }
}
