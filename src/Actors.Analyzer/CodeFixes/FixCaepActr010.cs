using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr010 : FixingActionProvider<ClassDeclarationSyntax>
{
  private const string s_title = "Make '{0}' concrete";

  protected override string DiagnosticId => CAEPACTR010;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, ClassDeclarationSyntax @class, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    SyntaxToken abstractKeywordToken = @class.Modifiers.FirstOrDefault(modifier => modifier.IsKind(SyntaxKind.AbstractKeyword));
    if (abstractKeywordToken == default)
      return Fail($"{CAEPACTR010} was supposed to be reported on abstract classes.");

    return new(CodeAction.Create(
      title: string.Format(s_title, @class.Identifier.ValueText),
      createChangedDocument: cancellationToken => Task.FromResult(RemoveAbstractKeyword(document, root, @class, abstractKeywordToken)),
      equivalenceKey: CAEPACTR010));
  }

  private static Document RemoveAbstractKeyword(Document document, SyntaxNode root, ClassDeclarationSyntax @class, SyntaxToken abstractKeywordToken)
  {
    SyntaxTokenList modifiers = @class.Modifiers.Remove(abstractKeywordToken);
    ClassDeclarationSyntax newClass = @class.WithModifiers(modifiers);
    SyntaxNode newRoot = root.ReplaceNode(@class, newClass);

    return document.WithSyntaxRoot(newRoot);
  }
}
