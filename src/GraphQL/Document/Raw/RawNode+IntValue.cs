using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IIntValueNode
{
  int IIntValueNode.Value
  {
    get
    {
      Expect(TokenKind.Integer);

      var value = _lexer.ValueSpan;
      _lexer.MoveNext();

      return int.Parse(value);
    }
  }
}
