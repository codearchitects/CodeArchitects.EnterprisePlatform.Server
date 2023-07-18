using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IFragmentSpreadNode
{
  ReadOnlySpan<char> IFragmentSpreadNode.FragmentName
  {
    get
    {
      Expect(TokenKind.Name);

      ReadOnlySpan<char> value = _lexer.Value;
      _lexer.MoveNext();

      return value;
    }
  }

  IDirectiveListNode? IFragmentSpreadNode.DirectiveList
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.At)
        return null;

      return this;
    }
  }
}
