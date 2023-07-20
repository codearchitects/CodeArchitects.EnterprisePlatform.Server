using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IDocumentNode, IEnumerable<IDefinitionNode>, IEnumerator<IDefinitionNode>
{
  IEnumerable<IDefinitionNode> IDocumentNode.Definitions => this;

  IDefinitionNode IEnumerator<IDefinitionNode>.Current => this;

  IEnumerator<IDefinitionNode> IEnumerable<IDefinitionNode>.GetEnumerator()
  {
    SetIterator(IteratorKind.Definition);
    return this;
  }

  private bool DefinitionMoveNext()
  {
    return _lexer.TokenKind is not TokenKind.EndOfFile;
  }
}
