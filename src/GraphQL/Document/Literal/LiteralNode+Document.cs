using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode : IDocumentNode, IEnumerable<IDefinitionNode>, IEnumerator<IDefinitionNode>
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
