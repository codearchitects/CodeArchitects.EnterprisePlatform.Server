using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IArgumentNode
{
  ReadOnlySpan<char> IArgumentNode.Name
  {
    get
    {
      Expect(TokenKind.Name);

      var value = _lexer.ValueSpan;
      _lexer.MoveNext();

      return value;
    }
  }

  IValueNode IArgumentNode.Value
  {
    get
    {
      Expect(TokenKind.Colon);

      _lexer.MoveNext();

      return this;
    }
  }
}
