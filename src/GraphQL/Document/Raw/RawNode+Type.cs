using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : ITypeNode
{
  TypeNodeKind ITypeNode.TypeKind
  {
    get
    {
      if (_typeKinds.Count == 0)
      {
        PushTypes();
      }

      TypeNodeKind kind = _typeKinds.Pop();

      if (kind is TypeNodeKind.ListType)
      {
        Expect(TokenKind.LeftBracket);
        _lexer.MoveNext();
      }

      return kind;
    }
  }

  private void PushTypes()
  {
    GraphQLLexer lexer = _lexer;

    int bracketCount = 0;
    while (lexer.TokenKind is TokenKind.LeftBracket)
    {
      bracketCount++;
      lexer.MoveNext();
    }

    RawGraphDocument.Expect(in lexer, TokenKind.Name);
    _typeKinds.Push(TypeNodeKind.NamedType);
    lexer.MoveNext();
    TypeNodeKind lastTypeKind = TypeNodeKind.NamedType;

    while (true)
    {
      switch (lexer.TokenKind)
      {
        case TokenKind.RightBracket:
          bracketCount--;
          lastTypeKind = TypeNodeKind.ListType;
          break;

        case TokenKind.Bang:
          if (lastTypeKind is TypeNodeKind.NonNullType)
            throw RawGraphDocument.Unexpected(in lexer);

          lastTypeKind = TypeNodeKind.NonNullType;
          break;

        default:
          if (bracketCount != 0)
            throw RawGraphDocument.Unexpected(in lexer);

          return;
      }

      _typeKinds.Push(lastTypeKind);
      lexer.MoveNext();
    }
  }
}
