using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  IEnumerable<IDirectiveNode> IDirectiveListNode.Directives => this;

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => this;

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator()
  {
    Debug.Assert(_lexer.TokenKind is TokenKind.At);

    SetIterator(IteratorKind.Directive);
    return this;
  }

  private bool DirectiveMoveNext()
  {
    if (_lexer.TokenKind is TokenKind.At)
    {
      _lexer.MoveNext();
      return true;
    }

    return false;
  }
}
