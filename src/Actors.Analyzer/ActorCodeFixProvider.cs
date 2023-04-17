using CodeArchitects.Platform.Actors.Analyzer.CodeFixes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Immutable;

namespace CodeArchitects.Platform.Actors.Analyzer;

[ExportCodeFixProvider(LanguageNames.CSharp)]
internal class ActorCodeFixProvider : CodeFixProvider
{
  public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(FixingActionProvider.FixableDiagnosticIds);

  public override FixAllProvider? GetFixAllProvider() => null;

  public override async Task RegisterCodeFixesAsync(CodeFixContext context)
  {
    Document document = context.Document;
    SyntaxNode? root = await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
    if (root is null)
      return;

    foreach (Diagnostic diagnostic in context.Diagnostics)
    {
      CodeAction? action = await FixingActionProvider.GetFixingActionAsync(document, root, diagnostic, context.CancellationToken).ConfigureAwait(false);
      if (action is null)
        continue;

      context.RegisterCodeFix(action, diagnostic);
    }
  }
}
