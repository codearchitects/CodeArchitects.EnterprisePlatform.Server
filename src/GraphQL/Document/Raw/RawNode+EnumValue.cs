using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IEnumValueNode
{
  string IEnumValueNode.Value
  {
    get
    {
      Expect(TokenKind.Name);

      var value = _lexer.ValueSpan;
      _lexer.MoveNext();

      if (value is "true" or "false" or "null")
        throw new InvalidOperationException("Attempted to read a non-enum value node as a enum value.");

      return value.ToString();
    }
  }
}
