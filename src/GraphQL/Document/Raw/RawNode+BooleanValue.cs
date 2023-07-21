using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IBooleanValueNode
{
  bool IBooleanValueNode.Value
  {
    get
    {
      Expect(TokenKind.Name);

      var value = _lexer.ValueSpan;
      _lexer.MoveNext();

      return value switch
      {
        "true"  => true,
        "false" => false,
        _       => throw new InvalidOperationException("Attempted to read a non-boolean value node as a boolean value.")
      };
    }
  }
}
