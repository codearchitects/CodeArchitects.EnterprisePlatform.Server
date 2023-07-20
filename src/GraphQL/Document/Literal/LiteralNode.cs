using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal partial class LiteralNode
{
  private GraphQLLexer _lexer;
  private readonly Stack<IteratorKind> _iteratorKinds;
  private readonly Stack<TypeNodeKind> _typeKinds;
  private bool _consumedOperationDefinition;

  public LiteralNode(string document)
  {
    _lexer = new GraphQLLexer(document);
    _iteratorKinds = new();
    _typeKinds = new();

    _lexer.MoveNext();
  }

  private IteratorKind CurrentIterator => _iteratorKinds.TryPeek(out IteratorKind kind) ? kind : IteratorKind.None;

  bool IEnumerator.MoveNext()
  {
    // Since IEnumerator<T>.MoveNext() does not exist, we have a shared MoveNext method.
    // We switch over the current iterator kind to simulate the desired behavior.
    // A stack is used to keep track of nested iterations between different document nodes.

    switch (CurrentIterator)
    {
      case IteratorKind.Definition:
        return DefinitionMoveNext();
      case IteratorKind.Variable:
        return VariableMoveNext();
      case IteratorKind.Directive:
        return DirectiveMoveNext();
      case IteratorKind.Argument:
        return ArgumentMoveNext();
      case IteratorKind.Selection:
        return SelectionMoveNext();
    }

    Debug.Fail("Invalid iteration over a GraphQL document node.");
    return false;
  }

  void IDisposable.Dispose()
  {
    _iteratorKinds.Pop();
  }

  private void SetIterator(IteratorKind kind)
  {
    _iteratorKinds.Push(kind);
  }

  private TokenKind PeekNext()
  {
    GraphQLLexer lexer = _lexer;
    lexer.MoveNext();
    return lexer.TokenKind;
  }

  private void Expect(TokenKind expected)
  {
    LiteralGraphDocument.Expect(in _lexer, expected);
  }

  private enum IteratorKind
  {
    None,
    Definition,
    Variable,
    Directive,
    Argument,
    Selection
  }

  #region Not relevant

  [ExcludeFromCodeCoverage]
  object IEnumerator.Current => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  void IEnumerator.Reset() => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  public IEnumerator GetEnumerator() => throw new NotSupportedException();

  #endregion
}
