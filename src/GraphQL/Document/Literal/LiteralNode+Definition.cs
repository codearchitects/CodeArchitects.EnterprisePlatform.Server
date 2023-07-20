using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IDefinitionNode
{
  DefinitionNodeKind IDefinitionNode.DefinitionKind
  {
    get
    {
      if (!_consumedOperationDefinition)
      {
        _consumedOperationDefinition = true;
        return DefinitionNodeKind.OperationDefinition;
      }

      Expect(TokenKind.Name);
      var value = _lexer.Value;

      if (value is "fragment")
      {
        _lexer.MoveNext();
        return DefinitionNodeKind.FragmentDefinition;
      }

      throw new InvalidGraphQLDocumentException("Expected a fragment definition.");
    }
  }
}
