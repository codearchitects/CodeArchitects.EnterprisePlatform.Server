using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr000 : FixingActionProvider<ClassDeclarationSyntax>
{
  private const string s_title = "Make the class non-generic";

  protected override string DiagnosticId => CAEPACTR000;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, ClassDeclarationSyntax @class, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    if (@class.TypeParameterList is not TypeParameterListSyntax typeParameterList)
      return Fail($"{CAEPACTR000} was supposed to be reported on generic classes.");

    return new(CodeAction.Create(
      title: s_title,
      createChangedDocument: cancellationToken => Task.FromResult(RemoveTypeParameterList(document, root, @class, typeParameterList)),
      equivalenceKey: CAEPACTR000));
  }

  private static Document RemoveTypeParameterList(Document document, SyntaxNode root, ClassDeclarationSyntax @class, TypeParameterListSyntax typeParameterList)
  {
    SyntaxTriviaList trailingTrivia = typeParameterList.GetTrailingTrivia();
    SyntaxToken identifier = @class.Identifier.WithTrailingTrivia(trailingTrivia);

    ClassDeclarationSyntax newClass = @class
      .WithTypeParameterList(null)
      .WithIdentifier(identifier)
      .WithConstraintClauses(new SyntaxList<TypeParameterConstraintClauseSyntax>());

    return document.WithSyntaxRoot(root.ReplaceNode(@class, newClass));
  }
}
