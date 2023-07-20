using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  IEnumerable<IDirectiveNode> IDirectiveListNode.Directives => this;

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => this;

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator()
  {
    Expect(TokenKind.At);

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
