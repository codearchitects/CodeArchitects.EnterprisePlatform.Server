using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IVariableDefinitionNode
{
  IVariableNode IVariableDefinitionNode.Variable => this;

  ITypeNode IVariableDefinitionNode.Type
  {
    get
    {
      Expect(TokenKind.Colon);
      _lexer.MoveNext();

      return this;
    }
  }
}
