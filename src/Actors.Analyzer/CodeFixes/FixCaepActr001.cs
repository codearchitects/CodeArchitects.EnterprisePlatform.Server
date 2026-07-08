using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr001 : FixingActionProvider<ClassDeclarationSyntax>
{
  private const string s_title = "Implement '{0}'";

  protected override string DiagnosticId => CAEPACTR001;

  protected override ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, ClassDeclarationSyntax @class, IReadOnlyList<string> properties, CancellationToken cancellationToken)
  {
    Project project = document.Project;
    if (!project.SupportsCompilation)
      return None;

    return new(GetFixingActionCoreAsync(document, root, @class, cancellationToken));
  }

  private async Task<CodeAction?> GetFixingActionCoreAsync(Document document, SyntaxNode root, ClassDeclarationSyntax @class, CancellationToken cancellationToken)
  {
    Project project = document.Project;
    Compilation? compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
    if (compilation is null)
      return null;

    string interfaceName = $"I{@class.Identifier.Text}";

    BaseNamespaceDeclarationSyntax? classNamespaceDeclaration = @class.GetNamespaceDeclaration();
    string interfaceQualifiedName = classNamespaceDeclaration is null
      ? interfaceName
      : $"{classNamespaceDeclaration.Name}.{interfaceName}";

    INamedTypeSymbol? existingInterfaceTypeSymbol = compilation.GetTypeByMetadataName(interfaceQualifiedName);

    string title = string.Format(s_title, interfaceName);

    if (existingInterfaceTypeSymbol is not null)
    {
      return CodeAction.Create(
        title: title,
        createChangedDocument: cancellationToken => Task.FromResult(AddInterfaceImplementation(document, root, @class, interfaceName)),
        equivalenceKey: CAEPACTR001);
    }

    if (classNamespaceDeclaration is not null)
    {
      return CodeAction.Create(
        title: title,
        createChangedSolution: cancellationToken => Task.FromResult(AddInterfaceDocument(document, root, @class, classNamespaceDeclaration, interfaceName)),
        equivalenceKey: CAEPACTR001);
    }

    return CodeAction.Create(
      title: title,
      createChangedDocument: cancellationToken => Task.FromResult(AddInterfaceImplementationAndDeclaration(document, root, @class, interfaceName)),
      equivalenceKey: CAEPACTR001);
  }

  private static Document AddInterfaceImplementation(Document document, SyntaxNode root, ClassDeclarationSyntax @class, string interfaceName)
  {
    ClassDeclarationSyntax newClass = AddToBaseList(@class, SyntaxFactory.IdentifierName(interfaceName));
    SyntaxNode newRoot = root.ReplaceNode(@class, newClass);
    return document.WithSyntaxRoot(newRoot);
  }

  private static Document AddInterfaceImplementationAndDeclaration(Document document, SyntaxNode root, ClassDeclarationSyntax @class, string interfaceName)
  {
    InterfaceDeclarationSyntax interfaceDeclaration = CreateInterfaceDeclaration(interfaceName, @class);
    ClassDeclarationSyntax newClass = AddToBaseList(@class, SyntaxFactory.IdentifierName(interfaceName)).WithTrailingTrivia(SyntaxFactory.LineFeed);

    CompilationUnitSyntax compilationUnit = (CompilationUnitSyntax)root;
    SyntaxList<MemberDeclarationSyntax> members = SyntaxFactory.List(compilationUnit.Members.ReplaceRange(@class, new MemberDeclarationSyntax[] { newClass, interfaceDeclaration }));
    SyntaxNode newRoot = compilationUnit.WithMembers(members);

    return document.WithSyntaxRoot(newRoot);
  }

  private static Solution AddInterfaceDocument(Document document, SyntaxNode root, ClassDeclarationSyntax @class, BaseNamespaceDeclarationSyntax classNamespaceDeclaration, string interfaceName)
  {
    Project project = document.Project;
    Solution solution = project.Solution;

    ClassDeclarationSyntax newClass = AddToBaseList(@class, SyntaxFactory.IdentifierName(interfaceName));
    SyntaxNode newRoot = root.ReplaceNode(@class, newClass);
    SyntaxNode interfaceFileRoot = CreateInterfaceDocumentRoot(classNamespaceDeclaration, interfaceName, @class);

    return solution
      .WithDocumentSyntaxRoot(
        documentId: document.Id,
        root: newRoot)
      .AddDocument(
        documentId: DocumentId.CreateNewId(project.Id),
        name: $"{interfaceName}.cs",
        syntaxRoot: interfaceFileRoot,
        folders: document.Folders);
  }

  private static SyntaxNode CreateInterfaceDocumentRoot(BaseNamespaceDeclarationSyntax classNamespaceDeclaration, string interfaceName, ClassDeclarationSyntax @class)
  {
    InterfaceDeclarationSyntax interfaceDeclaration = CreateInterfaceDeclaration(interfaceName, @class);

    BaseNamespaceDeclarationSyntax namespaceDeclaration = classNamespaceDeclaration
      .WithoutLeadingTrivia()
      .WithMembers(new SyntaxList<MemberDeclarationSyntax>(interfaceDeclaration));

    return SyntaxFactory.CompilationUnit(
      externs: new SyntaxList<ExternAliasDirectiveSyntax>(),
      usings: new SyntaxList<UsingDirectiveSyntax>(),
      attributeLists: new SyntaxList<AttributeListSyntax>(),
      members: new SyntaxList<MemberDeclarationSyntax>(namespaceDeclaration));
  }

  private static InterfaceDeclarationSyntax CreateInterfaceDeclaration(string interfaceName, ClassDeclarationSyntax @class)
  {
    return SyntaxFactory.InterfaceDeclaration(
      attributeLists: new SyntaxList<AttributeListSyntax>(),
      modifiers: SyntaxTokenList.Create(SyntaxFactory.Token(
        leading: @class.AttributeLists[0].GetLeadingTrivia(),
        kind: SyntaxKind.PublicKeyword,
        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
      keyword: SyntaxFactory.Token(
        leading: @class.Keyword.LeadingTrivia,
        kind: SyntaxKind.InterfaceKeyword,
        trailing: @class.Keyword.TrailingTrivia),
      identifier: SyntaxFactory.Identifier(
        leading: @class.Identifier.LeadingTrivia,
        text: interfaceName,
        trailing: @class.Identifier.TrailingTrivia),
      typeParameterList: null,
      baseList: null,
      constraintClauses: new SyntaxList<TypeParameterConstraintClauseSyntax>(),
      openBraceToken: @class.OpenBraceToken,
      members: new SyntaxList<MemberDeclarationSyntax>(),
      closeBraceToken: @class.CloseBraceToken,
      semicolonToken: default);
  }
}
