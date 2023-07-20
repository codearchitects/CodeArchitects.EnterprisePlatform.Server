using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IVariableDefinitionListNode, IEnumerable<IVariableDefinitionNode>, IEnumerator<IVariableDefinitionNode>
{
  IEnumerable<IVariableDefinitionNode> IVariableDefinitionListNode.VariableDefinitions => this;

  IVariableDefinitionNode IEnumerator<IVariableDefinitionNode>.Current => this;

  IEnumerator<IVariableDefinitionNode> IEnumerable<IVariableDefinitionNode>.GetEnumerator()
  {
    Debug.Assert(_lexer.TokenKind is TokenKind.LeftParenthesis);

    _lexer.MoveNext();
    if (_lexer.TokenKind is TokenKind.RightParenthesis)
      throw LiteralGraphDocument.Unexpected(in _lexer); // Validate against a "()" syntax

    SetIterator(IteratorKind.Variable);
    return this;
  }

  private bool VariableMoveNext()
  {
    if (_lexer.TokenKind is not TokenKind.RightParenthesis)
    {
      _lexer.MoveNext();
      return true;
    }

    _lexer.MoveNext();
    return false;
  }
}
