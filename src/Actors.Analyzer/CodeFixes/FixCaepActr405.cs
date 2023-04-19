using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr405 : FixingActionProvider<ParameterSyntax>
{
  private const string s_title = "Fix method signature";

  protected override string DiagnosticId => CAEPACTR405;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, ParameterSyntax parameter, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    Project project = document.Project;
    if (!project.SupportsCompilation)
      return None;

    return new(GetFixingActionCoreAsync(document, root, parameter, cancellationToken));
  }

  private static async Task<CodeAction?> GetFixingActionCoreAsync(Document document, SyntaxNode root, ParameterSyntax parameter, CancellationToken cancellationToken)
  {
    Project project = document.Project;
    Compilation? compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
    if (compilation is null)
      return null;

    INamedTypeSymbol? cancellationTokenTypeSymbol = compilation.GetTypeByMetadataName("System.Threading.CancellationToken");
    if (cancellationTokenTypeSymbol is null)
      return null;

    MethodDeclarationSyntax? method = parameter.FirstAncestorOrSelf<MethodDeclarationSyntax>();
    if (method is null)
      return null;

    SemanticModel semanticModel = compilation.GetSemanticModel(root.SyntaxTree);
    if (semanticModel.GetDeclaredSymbol(method, cancellationToken) is not IMethodSymbol methodSymbol)
      return null;

    IMethodSymbol? interfaceMethodSymbol = GetInterfaceMethodSymbol(methodSymbol);
    MethodDeclarationSyntax? interfaceMethod = interfaceMethodSymbol is null
      ? null
      : await interfaceMethodSymbol.DeclaringSyntaxReferences[0].GetSyntaxAsync(cancellationToken).ConfigureAwait(false) as MethodDeclarationSyntax;

    SignatureFixer fixer = new(parameter, methodSymbol, cancellationTokenTypeSymbol);

    if (interfaceMethod is null)
    {
      return CodeAction.Create(
        title: s_title,
        createChangedDocument: cancellationToken => Task.FromResult(FixMethodSignature(document, root, method, fixer)),
        equivalenceKey: CAEPACTR405);
    }

    return CodeAction.Create(
      title: s_title,
      createChangedSolution: cancellationToken => FixAllMethodSignaturesAsync(document, root, method, interfaceMethod, fixer, cancellationToken),
      equivalenceKey: CAEPACTR405);
  }

  private static Document FixMethodSignature(Document document, SyntaxNode root, MethodDeclarationSyntax methodDeclaration, SignatureFixer fixer)
  {
    return document.WithSyntaxRoot(fixer.FixSignature(root, methodDeclaration));
  }

  private static async Task<Solution> FixAllMethodSignaturesAsync(Document document, SyntaxNode actorRoot, MethodDeclarationSyntax actorMethodDeclaration, MethodDeclarationSyntax interfaceMethodDeclaration, SignatureFixer fixer, CancellationToken cancellationToken)
  {
    Project project = document.Project;
    Solution solution = project.Solution;
    Document? interfaceDocument = solution.GetDocument(interfaceMethodDeclaration.SyntaxTree);

    SyntaxNode newRoot = fixer.FixSignature(actorRoot, actorMethodDeclaration);
    solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);

    if (interfaceDocument is null)
      return solution;

    SyntaxNode? interfaceRoot = await interfaceDocument.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
    if (interfaceRoot is null)
      return solution;

    SyntaxNode newInterfaceRoot = fixer.FixSignature(interfaceRoot, interfaceMethodDeclaration);
    solution = solution.WithDocumentSyntaxRoot(interfaceDocument.Id, newInterfaceRoot);

    return solution;
  }

  private class SignatureFixer
  {
    private readonly ParameterSyntax _cancellationTokenParameter;
    private readonly List<int> _indices;

    public SignatureFixer(ParameterSyntax cancellationTokenParameter, IMethodSymbol methodSymbol, INamedTypeSymbol cancellationTokenTypeSymbol)
    {
      _cancellationTokenParameter = cancellationTokenParameter;
      _indices = new();

      ImmutableArray<IParameterSymbol> parameterSymbols = methodSymbol.Parameters;
      for (int i = 0; i < parameterSymbols.Length; i++)
      {
        IParameterSymbol parameterSymbol = parameterSymbols[i];
        if (!SymbolEqualityComparer.Default.Equals(parameterSymbol.Type, cancellationTokenTypeSymbol))
        {
          _indices.Add(i);
        }
      }
    }

    public SyntaxNode FixSignature(SyntaxNode root, MethodDeclarationSyntax methodDeclaration)
    {
      List<ParameterSyntax> parameterList = new();
      SeparatedSyntaxList<ParameterSyntax> parameters = methodDeclaration.ParameterList.Parameters;

      foreach (int index in _indices)
      {
        parameterList.Add(parameters[index]);
      }

      parameterList.Add(_cancellationTokenParameter);
      SeparatedSyntaxList<ParameterSyntax> newParameters = SyntaxFactory.SeparatedList(parameterList, _indices.Select(_ => SyntaxFactory.Token(
        leading: SyntaxFactory.TriviaList(),
        kind: SyntaxKind.CommaToken,
        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))));

      return root.ReplaceNode(
        oldNode: methodDeclaration,
        newNode: methodDeclaration.WithParameterList(methodDeclaration.ParameterList.WithParameters(newParameters)));
    }
  }
}