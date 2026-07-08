using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IDirectiveListNode, IEnumerable<IDirectiveNode>, IEnumerator<IDirectiveNode>
{
  IEnumerable<IDirectiveNode> IDirectiveListNode.Directives
  {
    get
    {
      Expect(TokenKind.At);

      return this;
    }
  }

  IDirectiveNode IEnumerator<IDirectiveNode>.Current => this;

  IEnumerator<IDirectiveNode> IEnumerable<IDirectiveNode>.GetEnumerator()
  {
    SetIterator(IteratorKind.Directive);
    return this;
  }

  private bool DirectiveMoveNext()
  {
    return _lexer.TokenKind is TokenKind.At;
  }
}
