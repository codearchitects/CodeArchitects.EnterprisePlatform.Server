using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IFieldNode
{
  ReadOnlySpan<char> IFieldNode.Alias
  {
    get
    {
      if (PeekNext() is not TokenKind.Colon)
        return ReadOnlySpan<char>.Empty;

      Expect(TokenKind.Name);

      ReadOnlySpan<char> value = _lexer.Value;
      _lexer.MoveNext();

      Debug.Assert(_lexer.TokenKind is TokenKind.Colon);
      _lexer.MoveNext();

      return value;
    }
  }

  ReadOnlySpan<char> IFieldNode.FieldName
  {
    get
    {
      Expect(TokenKind.Name);

      ReadOnlySpan<char> value = _lexer.Value;
      _lexer.MoveNext();

      return value;
    }
  }

  IArgumentListNode? IFieldNode.ArgumentList
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.LeftParenthesis)
        return null;

      return this;
    }
  }

  IDirectiveListNode? IFieldNode.DirectiveList
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.At)
        return null;

      return this;
    }
  }

  ISelectionSetNode? IFieldNode.SelectionSet
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.LeftBrace)
        return null;

      return this;
    }
  }
}
