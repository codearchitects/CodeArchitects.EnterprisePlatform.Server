using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IFragmentDefinitionNode
{
  ReadOnlySpan<char> IFragmentDefinitionNode.FragmentName
  {
    get
    {
      Expect(TokenKind.Name);

      ReadOnlySpan<char> value = _lexer.Value;

      if (value is "on")
        throw new InvalidGraphQLDocumentException($"Unexpected fragment name 'on'.");

      _lexer.MoveNext();

      return value;
    }
  }

  ITypeConditionNode IFragmentDefinitionNode.TypeCondition
  {
    get
    {
      Expect(TokenKind.Name);
      if (_lexer.Value is not "on")
        throw new InvalidGraphQLDocumentException($"Expected 'on', found '{_lexer.Value.ToString()}'");

      _lexer.MoveNext();
      return this;
    }
  }

  IDirectiveListNode? IFragmentDefinitionNode.DirectiveList
  {
    get
    {
      if (_lexer.TokenKind is not TokenKind.At)
        return null;

      return this;
    }
  }

  ISelectionSetNode IFragmentDefinitionNode.SelectionSet
  {
    get
    {
      Expect(TokenKind.LeftBrace);
      return this;
    }
  }
}
