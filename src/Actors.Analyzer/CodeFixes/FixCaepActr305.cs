using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr305 : FixingActionProvider<VariableDeclaratorSyntax>
{
  private const string s_title = "Make '{0}' of type '{1}'";

  protected override string DiagnosticId => CAEPACTR305;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, VariableDeclaratorSyntax variableDeclarator, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    Project project = document.Project;
    if (!project.SupportsCompilation)
      return None;

    if (properties.Count != 3)
      return None;

    return new(GetFixingActionCoreAsync(document, root, variableDeclarator, properties, cancellationToken));
  }

  private static async Task<CodeAction?> GetFixingActionCoreAsync(Document document, SyntaxNode root, VariableDeclaratorSyntax variableDeclarator, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    Project project = document.Project;
    Compilation? compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
    if (compilation is null)
      return null;

    SyntaxTree? idSyntaxTree = compilation.SyntaxTrees.FirstOrDefault(tree => tree.FilePath == properties[0]);
    if (idSyntaxTree is null)
      return null;

    VariableDeclarationSyntax invalidDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent!;
    TypeSyntax invalidIdType = invalidDeclaration.Type;
    
    SemanticModel semanticModel = compilation.GetSemanticModel(root.SyntaxTree);
    SemanticModel idSemanticModel = compilation.GetSemanticModel(idSyntaxTree);

    TextSpan idReferenceTextSpan = new(int.Parse(properties[1]), int.Parse(properties[2]));

    SyntaxNode idSyntaxRoot = await idSyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
    SyntaxNode idReferenceNode = idSyntaxRoot.FindNode(idReferenceTextSpan);

    TypeSyntax? idType = idReferenceNode switch
    {
      AttributeSyntax attribute             => attribute.GetTarget(attribute.Name),
      VariableDeclaratorSyntax idDeclarator => ((VariableDeclarationSyntax)idDeclarator.Parent!).Type,
      _                                     => null
    };

    if (idType is null)
      return null;

    if (idSemanticModel.GetSymbolInfo(idType).Symbol is not INamedTypeSymbol idTypeSymbol)
      return null;

    if (idTypeSymbol.IsGenericType || idTypeSymbol.ContainingType is not null)
      return null;

    return CodeAction.Create(
      title: string.Format(s_title, variableDeclarator.Identifier.ValueText, idTypeSymbol.Name),
      createChangedDocument: cancellationToken => Task.FromResult(ChangeIdType(document, root, invalidIdType, idType, semanticModel, idTypeSymbol)),
      equivalenceKey: CAEPACTR305);
  }

  private static Document ChangeIdType(Document document, SyntaxNode root, TypeSyntax invalidIdType, TypeSyntax idType, SemanticModel semanticModel, INamedTypeSymbol idTypeSymbol)
  {
    TypeSyntax newIdType = GetNewIdType(invalidIdType, idType, semanticModel, idTypeSymbol);

    SyntaxNode newRoot = root.ReplaceNode(invalidIdType, newIdType.WithTrailingTrivia(invalidIdType.GetTrailingTrivia()));

    return document.WithSyntaxRoot(newRoot);

    static TypeSyntax GetNewIdType(TypeSyntax invalidIdType, TypeSyntax idType, SemanticModel semanticModel, INamedTypeSymbol idTypeSymbol)
    {
      if (idTypeSymbol.IsAccessible(semanticModel, invalidIdType.SpanStart))
        return idType;

      string[] parts = idTypeSymbol.ToDisplayString(Format.FullName).Split('.');
      NameSyntax qualifiedName = SyntaxFactory.IdentifierName(parts[0]);
      for (int i = 1; i < parts.Length; i++)
      {
        qualifiedName = SyntaxFactory.QualifiedName(qualifiedName, SyntaxFactory.IdentifierName(parts[i]));
      }

      return qualifiedName;
    }
  }
}