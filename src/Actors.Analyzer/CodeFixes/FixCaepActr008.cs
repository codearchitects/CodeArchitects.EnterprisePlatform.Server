using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using static CodeArchitects.Platform.Actors.Analyzer.DiagnosticIds;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal class FixCaepActr008 : FixingActionProvider<ClassDeclarationSyntax>
{
  private const string s_title = "Choose a default implementation";
  private const string s_nestedTitle = "Make '{0}' the default implementation";

  protected override string DiagnosticId => CAEPACTR008;

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

    SemanticModel semanticModel = compilation.GetSemanticModel(root.SyntaxTree);
    if (semanticModel.GetDeclaredSymbol(@class, cancellationToken) is not INamedTypeSymbol actorType)
      return null;

    List<ImplementationData> implementationDataCollection = ImplementationFinder.FindImplementations(actorType, compilation.Assembly);

    IEnumerable<CodeAction> actions = implementationDataCollection.Select((implementationData, index) => CodeAction.Create(
      title: string.Format(s_nestedTitle, implementationData.ImplementationType.Name),
      createChangedSolution: cancellationToken => MakeDefaultImplementationAsync(project.Solution, implementationDataCollection, index, cancellationToken),
      equivalenceKey: CAEPACTR008));

    return CodeAction.Create(
      title: s_title,
      nestedActions: ImmutableArray.CreateRange(actions),
      isInlinable: false);
  }

  private static async Task<Solution> MakeDefaultImplementationAsync(Solution solution, List<ImplementationData> implementationDataCollection, int defaultIndex, CancellationToken cancellationToken)
  {
    Dictionary<Document, List<AttributeSyntax>> attributes = new();

    for (int i = 0; i < implementationDataCollection.Count; i++)
    {
      if (i == defaultIndex)
        continue;

      ImplementationData implementationData = implementationDataCollection[i];
      AttributeData implementationAttribute = implementationData.ImplementationAttribute;

      if (implementationAttribute.ApplicationSyntaxReference is not SyntaxReference syntaxReference)
        continue;

      AttributeSyntax attribute = (AttributeSyntax)await syntaxReference.GetSyntaxAsync(cancellationToken).ConfigureAwait(false);
      if (solution.GetDocument(attribute.SyntaxTree) is not Document document)
        continue;

      if (!attributes.TryGetValue(document, out List<AttributeSyntax> documentAttributes))
      {
        documentAttributes = new();
        attributes[document] = documentAttributes;
      }

      documentAttributes.Add(attribute);
    }

    foreach (KeyValuePair<Document, List<AttributeSyntax>> entries in attributes)
    {
      Document document = entries.Key;
      List<AttributeSyntax> documentAttributes = entries.Value;

      SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
      if (root is null)
        continue;

      SyntaxNode newRoot = root.ReplaceNodes(documentAttributes, (originalNode, replacedNode) =>
      {
        return originalNode.WithArgumentList(null);
      });

      solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);
    }

    return solution;
  }

  internal class ImplementationFinder : SymbolVisitor
  {
    private readonly INamedTypeSymbol _actorType;
    private readonly ICollection<ImplementationData> _implementationDataCollection;

    public ImplementationFinder(INamedTypeSymbol actorType, ICollection<ImplementationData> implementationDataCollection)
    {
      _actorType = actorType;
      _implementationDataCollection = implementationDataCollection;
    }

    public override void VisitNamespace(INamespaceSymbol symbol)
    {
      foreach (INamespaceOrTypeSymbol member in symbol.GetMembers())
      {
        member.Accept(this);
      }
    }

    public override void VisitNamedType(INamedTypeSymbol symbol)
    {
      if (IsActorDefaultImplementation(symbol, out AttributeData? implementationAttribute))
      {
        _implementationDataCollection.Add(new ImplementationData(symbol, implementationAttribute));
      }
    }

    private bool IsActorDefaultImplementation(INamedTypeSymbol type, [NotNullWhen(true)] out AttributeData? implementationAttribute)
    {
      implementationAttribute = null;

      foreach (AttributeData attribute in type.GetAttributes())
      {
        if (attribute.AttributeClass is not INamedTypeSymbol attributeClass)
          continue;

        if (!attributeClass.ContainingNamespace.IsCodeArchitectsPlatformActorsNamespace())
          continue;

        if (attributeClass.Name != "ActorImplementationAttribute")
          continue;

        if (!attribute.TryGetAttributeTargetType(out ITypeSymbol? targetType))
          continue;

        if (!SymbolEqualityComparer.Default.Equals(_actorType, targetType))
          continue;

        foreach (KeyValuePair<string, TypedConstant> arg in attribute.NamedArguments)
        {
          if (arg.Key != "IsDefault" || arg.Value.Value is not bool argumentValue || !argumentValue)
            continue;

          implementationAttribute = attribute;
          return true;
        }

        return false;
      }

      return false;
    }

    public static List<ImplementationData> FindImplementations(INamedTypeSymbol actorType, IAssemblySymbol assembly)
    {
      List<ImplementationData> implementationDataCollection = new();
      new ImplementationFinder(actorType, implementationDataCollection).VisitNamespace(assembly.GlobalNamespace);
      return implementationDataCollection;
    }
  }

  internal readonly record struct ImplementationData(INamedTypeSymbol ImplementationType, AttributeData ImplementationAttribute);
}
