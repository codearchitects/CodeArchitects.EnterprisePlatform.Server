using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IFloatValueNode
{
  double IFloatValueNode.Value
  {
    get
    {
      Expect(TokenKind.Float);

      var value = _lexer.ValueSpan;
      _lexer.MoveNext();

      return double.Parse(value);
    }
  }
}
