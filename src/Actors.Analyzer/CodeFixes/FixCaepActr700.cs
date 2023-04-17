using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal sealed class FixCaepActr700 : FixingActionProvider<AttributeSyntax>
{
  private const string s_title = "Remove extra attribute";

  protected override string DiagnosticId => CAEPACTR700;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, AttributeSyntax attribute, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    if (attribute.Parent is not AttributeListSyntax attributeList)
      return None;

    if (attributeList.Attributes.Count > 1)
    {
      return new(CodeAction.Create(
        title: s_title,
        createChangedDocument: cancellationToken => Task.FromResult(RemoveAttributeFromAttributeList(document, root, attribute, attributeList)),
        equivalenceKey: CAEPACTR700));
    }

    if (attributeList.Parent is not ClassDeclarationSyntax @class)
      return None;

    return new(CodeAction.Create(
      title: s_title,
      createChangedDocument: cancellationToken => Task.FromResult(RemoveAttributeList(document, root, attributeList, @class)),
      equivalenceKey: CAEPACTR700));
  }

  private static Document RemoveAttributeFromAttributeList(Document document, SyntaxNode root, AttributeSyntax attribute, AttributeListSyntax attributeList)
  {
    AttributeListSyntax newAttributeList = SyntaxFactory.AttributeList(attributeList.Attributes.Remove(attribute));

    return document.WithSyntaxRoot(root.ReplaceNode(attributeList, newAttributeList));
  }

  private static Document RemoveAttributeList(Document document, SyntaxNode root, AttributeListSyntax attributeList, ClassDeclarationSyntax @class)
  {
    SyntaxTriviaList leadingTrivia = @class.GetLeadingTrivia();
    SyntaxList<AttributeListSyntax> newAttributeLists = @class.AttributeLists.Remove(attributeList);

    ClassDeclarationSyntax newClass = @class
      .WithAttributeLists(newAttributeLists)
      .WithLeadingTrivia(leadingTrivia);

    return document.WithSyntaxRoot(root.ReplaceNode(@class, newClass));
  }
}
