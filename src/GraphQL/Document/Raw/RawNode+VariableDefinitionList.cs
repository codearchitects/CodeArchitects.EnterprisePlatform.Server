using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IVariableDefinitionListNode, IEnumerable<IVariableDefinitionNode>, IEnumerator<IVariableDefinitionNode>
{
  IEnumerable<IVariableDefinitionNode> IVariableDefinitionListNode.VariableDefinitions
  {
    get
    {
      Expect(TokenKind.LeftParenthesis);
      _lexer.MoveNext();

      return this;
    }
  }

  IVariableDefinitionNode IEnumerator<IVariableDefinitionNode>.Current => this;

  IEnumerator<IVariableDefinitionNode> IEnumerable<IVariableDefinitionNode>.GetEnumerator()
  {
    if (_lexer.TokenKind is TokenKind.RightParenthesis)
      throw Unexpected(); // Validate against a "()" syntax

    SetIterator(IteratorKind.VariableDefinition);
    return this;
  }

  private bool VariableDefinitionMoveNext()
  {
    if (_lexer.TokenKind is not TokenKind.RightParenthesis)
      return true;

    _lexer.MoveNext();
    return false;
  }
}
