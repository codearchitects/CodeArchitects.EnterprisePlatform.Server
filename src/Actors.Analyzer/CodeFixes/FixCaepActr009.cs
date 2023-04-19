using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr009 : FixingActionProvider<TypeSyntax>
{
  private const string s_title = "Inherit '{0}'";

  protected override string DiagnosticId => CAEPACTR009;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, TypeSyntax type, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    if (type.FirstAncestorOrSelf<ClassDeclarationSyntax>() is not { } @class)
      return Fail($"{CAEPACTR009} was supposed to be reported on a class.");

    if (@class.BaseList is not null) // TODO: If the base list consists of interfaces, we can provide a code fix
      return None;

    return new(CodeAction.Create(
      title: string.Format(s_title, type.ToString()),
      createChangedDocument: cancellationToken => Task.FromResult(AddBaseType(document, root, type, @class)),
      equivalenceKey: CAEPACTR009));
  }

  private static Document AddBaseType(Document document, SyntaxNode root, TypeSyntax type, ClassDeclarationSyntax @class)
  {
    ClassDeclarationSyntax newClass = AddToBaseList(@class, type);
    SyntaxNode newRoot = root.ReplaceNode(@class, newClass);

    return document.WithSyntaxRoot(newRoot);
  }
}
