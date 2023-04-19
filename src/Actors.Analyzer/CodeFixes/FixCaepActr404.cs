using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr404 : FixingActionProvider<MethodDeclarationSyntax>
{
  private const string s_title = "Make '{0}' return '{1}'";

  protected override string DiagnosticId => CAEPACTR404;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, MethodDeclarationSyntax method, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    Project project = document.Project;
    if (!project.SupportsCompilation)
      return None;

    if (root is not CompilationUnitSyntax compilationUnit)
      return None;

    return new(GetFixingActionCoreAsync(document, compilationUnit, method, cancellationToken));
  }

  private static async Task<CodeAction?> GetFixingActionCoreAsync(Document document, CompilationUnitSyntax root, MethodDeclarationSyntax method, CancellationToken cancellationToken)
  {
    Project project = document.Project;
    Compilation? compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
    if (compilation is null)
      return null;

    INamedTypeSymbol? taskTypeSymbol = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
    if (taskTypeSymbol is null)
    {
      Debug.Fail("Could not get Task symbol.");
      return null;
    }

    SemanticModel semanticModel = compilation.GetSemanticModel(root.SyntaxTree);
    if (semanticModel.GetDeclaredSymbol(method, cancellationToken) is not IMethodSymbol methodSymbol)
      return null;

    IMethodSymbol? interfaceMethodSymbol = GetInterfaceMethodSymbol(methodSymbol);
    MethodDeclarationSyntax? interfaceMethod = interfaceMethodSymbol is null
      ? null
      : await interfaceMethodSymbol.DeclaringSyntaxReferences[0].GetSyntaxAsync(cancellationToken).ConfigureAwait(false) as MethodDeclarationSyntax;

    TypeInfo returnTypeInfo = semanticModel.GetTypeInfo(method.ReturnType, cancellationToken);
    if (returnTypeInfo.Type is not ITypeSymbol returnTypeSymbol)
      return null;

    TypeSyntax newReturnType = returnTypeSymbol.SpecialType is SpecialType.System_Void
      ? SyntaxFactory.IdentifierName("Task")
      : SyntaxFactory.GenericName(
          identifier: SyntaxFactory.Identifier("Task"),
          typeArgumentList: SyntaxFactory.TypeArgumentList(
            arguments: SyntaxFactory.SingletonSeparatedList(method.ReturnType.WithoutTrivia())));

    ReturnTypeFixer fixer = new(newReturnType, taskTypeSymbol);

    if (interfaceMethod is null)
    {
      return CodeAction.Create(
        title: string.Format(s_title, methodSymbol.Name, newReturnType),
        createChangedDocument: cancellationToken => Task.FromResult(FixReturnType(document, semanticModel, root, method, fixer)),
        equivalenceKey: CAEPACTR405);
    }

    return CodeAction.Create(
      title: string.Format(s_title, methodSymbol.Name, newReturnType),
      createChangedSolution: cancellationToken => FixAllReturnTypesAsync(document, semanticModel, root, method, interfaceMethod, fixer, cancellationToken),
      equivalenceKey: CAEPACTR405);
  }

  private static Document FixReturnType(Document document, SemanticModel semanticModel, CompilationUnitSyntax root, MethodDeclarationSyntax method, ReturnTypeFixer fixer)
  {
    return document.WithSyntaxRoot(fixer.FixReturnType(semanticModel, root, method));
  }

  private static async Task<Solution> FixAllReturnTypesAsync(Document actorDocument, SemanticModel actorSemanticModel, CompilationUnitSyntax actorRoot, MethodDeclarationSyntax actorMethod, MethodDeclarationSyntax interfaceMethod, ReturnTypeFixer fixer, CancellationToken cancellationToken)
  {
    Project project = actorDocument.Project;
    Solution solution = project.Solution;
    Document? interfaceDocument = solution.GetDocument(interfaceMethod.SyntaxTree);

    solution = solution.WithDocumentSyntaxRoot(actorDocument.Id, fixer.FixReturnType(actorSemanticModel, actorRoot, actorMethod));

    if (interfaceDocument is null || !interfaceDocument.SupportsSemanticModel)
      return solution;

    SemanticModel? interfaceSemanticModel = await interfaceDocument.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
    if (interfaceSemanticModel is null)
      return solution;

    if (await interfaceDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not CompilationUnitSyntax interfaceRoot)
      return solution;

    solution = solution.WithDocumentSyntaxRoot(interfaceDocument.Id, fixer.FixReturnType(interfaceSemanticModel, interfaceRoot, interfaceMethod));

    return solution;
  }

  private class ReturnTypeFixer
  {
    private readonly TypeSyntax _newReturnType;
    private readonly INamedTypeSymbol _taskTypeSymbol;

    public ReturnTypeFixer(TypeSyntax newReturnType, INamedTypeSymbol taskTypeSymbol)
    {
      _newReturnType = newReturnType;
      _taskTypeSymbol = taskTypeSymbol;
    }

    public SyntaxNode FixReturnType(SemanticModel semanticModel, CompilationUnitSyntax root, MethodDeclarationSyntax method)
    {
      string methodName = method.Identifier.Text;
      MethodDeclarationSyntax newMethod = method.WithReturnType(_newReturnType.WithTriviaFrom(method.ReturnType));
      if (!methodName.EndsWith("Async"))
      {
        newMethod = newMethod.WithIdentifier(SyntaxFactory.Identifier(methodName + "Async"));
      }

      root = root.ReplaceNode(method, newMethod);

      bool isTaskAccessible = _taskTypeSymbol.IsAccessible(semanticModel, method.ReturnType.SpanStart);
      if (isTaskAccessible)
        return root;

      return root.AddUsings(SyntaxFactory.UsingDirective(
        usingKeyword: SyntaxFactory.Token(
          leading: default,
          kind: SyntaxKind.UsingKeyword,
          trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
        staticKeyword: default,
        alias: null,
        name: SyntaxFactory.QualifiedName(
          left: SyntaxFactory.QualifiedName(
            left: SyntaxFactory.IdentifierName("System"),
            right: SyntaxFactory.IdentifierName("Threading")),
          right: SyntaxFactory.IdentifierName("Tasks")),
        semicolonToken: SyntaxFactory.Token(
          leading: default,
          kind: SyntaxKind.SemicolonToken,
          trailing: SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed))));
    }
  }
}