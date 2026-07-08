using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IVariableDefinitionNode
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
