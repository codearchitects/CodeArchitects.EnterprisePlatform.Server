using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IValueNode, IRawLiteralNode
{
  ValueNodeKind IValueNode.ValueKind
  {
    get
    {
      return _lexer.TokenKind switch
      {
        TokenKind.Dollar                                      => ValueNodeKind.Variable,
        TokenKind.Integer                                     => ValueNodeKind.IntValue,
        TokenKind.Float                                       => ValueNodeKind.FloatValue,
        TokenKind.String                                      => ValueNodeKind.StringValue,
        TokenKind.BlockString                                 => ValueNodeKind.BlockStringValue,
        TokenKind.Name when _lexer.Value is "true" or "false" => ValueNodeKind.BooleanValue,
        TokenKind.Name when _lexer.Value is "null"            => ValueNodeKind.NullValue,
        TokenKind.Name                                        => ValueNodeKind.EnumValue,
        TokenKind.LeftBracket                                 => ValueNodeKind.ListValue,
        TokenKind.LeftBrace                                   => ValueNodeKind.ObjectValue,
        _                                                     => throw Unexpected()
      };
    }
  }

  ReadOnlySpan<char> IRawLiteralNode.ValueText
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.Integer and not TokenKind.Float and not TokenKind.String and not TokenKind.Name)
        throw new InvalidOperationException("Attempted to get the value text of a non-literal node.");

      var value = _lexer.Value;
      _lexer.MoveNext();

      return value;
    }
  }
}
