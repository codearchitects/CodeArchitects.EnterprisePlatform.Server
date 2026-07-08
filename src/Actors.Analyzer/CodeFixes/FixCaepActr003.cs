using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr003 : FixingActionProvider<TypeSyntax>
{
  private const string s_title = "Implement '{0}'";

  protected override string DiagnosticId => CAEPACTR003;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, TypeSyntax type, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    if (type.FirstAncestorOrSelf<ClassDeclarationSyntax>() is not { } @class)
      return Fail($"{CAEPACTR003} was supposed to be reported on class attributes.");

    return new(CodeAction.Create(
      title: string.Format(s_title, type),
      createChangedDocument: cancellationToken => Task.FromResult(AddInterfaceImplementation(document, root, @class, type)),
      equivalenceKey: CAEPACTR003));
  }

  private static Document AddInterfaceImplementation(Document document, SyntaxNode root, ClassDeclarationSyntax @class, TypeSyntax interfaceType)
  {
    ClassDeclarationSyntax newClass = AddToBaseList(@class, interfaceType);

    SyntaxNode newRoot = root.ReplaceNode(@class, newClass);
    return document.WithSyntaxRoot(newRoot);
  }
}
