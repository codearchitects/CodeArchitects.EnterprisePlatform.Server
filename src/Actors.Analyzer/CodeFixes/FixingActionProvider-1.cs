using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using System.Diagnostics;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal abstract class FixingActionProvider<TSyntaxNode> : FixingActionProvider
  where TSyntaxNode : SyntaxNode
{
  protected sealed override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, SyntaxNode node, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    if (node is not TSyntaxNode typedNode)
    {
      Debug.Fail($"Expected syntax node of type '{typeof(TSyntaxNode).Name}', but got '{node.GetType().Name}'.");
      return None;
    }

    return GetFixingActionAsync(document, root, typedNode, properties, cancellationToken);
  }

  protected abstract ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, TSyntaxNode node, IReadOnlyList<string> properties, CancellationToken cancellationToken);
}
