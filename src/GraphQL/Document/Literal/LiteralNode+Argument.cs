using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IArgumentNode
{
  ReadOnlySpan<char> IArgumentNode.Name
  {
    get
    {
      Expect(TokenKind.Name);

      var value = _lexer.Value;
      _lexer.MoveNext();

      return value;
    }
  }

  object? IArgumentNode.Value
  {
    get
    {
      // TODO: We can optimize for certain kinds of values (e.g., integers, floats, strings)
      // by returning an ILiteralValue(TokenKind Kind, string Value) which the document renderer can use,
      // without parsing the actual values

      Expect(TokenKind.Colon);

      _lexer.MoveNext();

      object? value;
      switch (_lexer.TokenKind)
      {
        case TokenKind.Integer:
          value = int.Parse(_lexer.Value);
          break;

        case TokenKind.Float:
          value = float.Parse(_lexer.Value);
          break;

        case TokenKind.String:
          value = _lexer.Value[1..^1].ToString();
          break;

        case TokenKind.LeftBracket:
          // TODO: List
          throw new NotImplementedException();

        case TokenKind.LeftBrace:
          // TODO: Object
          throw new NotImplementedException();

        case TokenKind.Dollar:
          // TODO: Variable
          throw new NotImplementedException();

        case TokenKind.BlockString:
          // TODO: Block string
          throw new NotImplementedException();

        case TokenKind.Name:
          // TODO: Name
          throw new NotImplementedException();

        default:
          throw InvalidGraphQLDocumentException.Unexpected(_lexer.TokenKind);
      }

      _lexer.MoveNext();
      return value;
    }
  }
}
