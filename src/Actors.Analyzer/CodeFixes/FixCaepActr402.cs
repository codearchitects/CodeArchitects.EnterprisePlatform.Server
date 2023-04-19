using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr402 : FixingActionProvider<ParameterSyntax>
{
  private const string s_title = "Make '{0}' of type 'IActorContext<{1}>'";

  protected override string DiagnosticId => CAEPACTR402;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, ParameterSyntax parameter, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    ClassDeclarationSyntax? @class = parameter.FirstAncestorOrSelf<ClassDeclarationSyntax>();
    if (@class is null)
      return None;

    if (parameter.Type is not GenericNameSyntax contextType)
      return Fail($"{CAEPACTR402} was supposed to be reported on a generic parameter");

    return new(CodeAction.Create(
      title: string.Format(s_title, parameter.Identifier.ValueText, @class.Identifier.ValueText),
      createChangedDocument: cancellationToken => Task.FromResult(FixActorContextType(document, root, contextType, @class)),
      equivalenceKey: CAEPACTR402));
  }

  private static Document FixActorContextType(Document document, SyntaxNode root, GenericNameSyntax contextType, ClassDeclarationSyntax @class)
  {
    GenericNameSyntax newContextType = contextType.WithTypeArgumentList(
      typeArgumentList: SyntaxFactory.TypeArgumentList(
        arguments: SyntaxFactory.SeparatedList(new TypeSyntax[] { SyntaxFactory.IdentifierName(@class.Identifier) })));

    root = root.ReplaceNode(contextType, newContextType);

    return document.WithSyntaxRoot(root);
  }
}