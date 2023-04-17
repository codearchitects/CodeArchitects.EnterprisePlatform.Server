using CodeArchitects.Platform.Actors.Analyzer.Generator;
using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace CodeArchitects.Platform.Actors.Analyzer;

[Generator]
internal class ActorSourceGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    var optionsProvider = context.CompilationProvider
      .Select(GetActorAnalyzerOptions);

    var typeDataProvider = context.SyntaxProvider
      .CreateSyntaxProvider(OfTypesWithAttributes, GetTypeData)
      .Where(static data => data is not null)
      .Collect();

    var resultProvider = typeDataProvider
      .Combine(optionsProvider)
      .Select(GetDescriptorResult!);

    context.RegisterSourceOutput(resultProvider, Execute);
  }

  private static void Execute(SourceProductionContext context, DescriptorResult result)
  {
    context.CancellationToken.ThrowIfCancellationRequested();

    (RecordList<ActorDescriptor> descriptors, RecordList<DiagnosticReference> diagnostics) = result;

    ActorFactoryRenderer factoryRenderer = new(context);
    foreach (ActorDescriptor descriptor in descriptors)
    {
      factoryRenderer.Render(descriptor);
    }

    DiagnosticReferenceRenderer diagnosticRenderer = new(context);
    diagnosticRenderer.Render(diagnostics);
  }

  private static bool OfTypesWithAttributes(SyntaxNode node, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    return node is
      ClassDeclarationSyntax { AttributeLists.Count: > 0 } or
      InterfaceDeclarationSyntax { AttributeLists.Count: > 0 };
  }

  private static TypeData? GetTypeData(GeneratorSyntaxContext context, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    SemanticModel semanticModel = context.SemanticModel;
    if (semanticModel.GetDeclaredSymbol(context.Node, cancellationToken) is not INamedTypeSymbol type)
      return null;

    AttributeData? actorAttribute = null;
    AttributeData? genericActorAttribute = null;
    AttributeData? virtualAttribute = null;
    AttributeData? factoryAttribute = null;
    AttributeData? genericFactoryAttribute = null;
    AttributeData? implementationAttribute = null;
    AttributeData? genericImplementationAttribute = null;
    AttributeData? idTypeAttribute = null;
    AttributeData? genericIdTypeAttribute = null;
    foreach (AttributeData attribute in type.GetAttributes())
    {
      if (attribute.AttributeClass is not INamedTypeSymbol attributeType)
        continue;

      if (!attributeType.ContainingNamespace.IsCodeArchitectsPlatformActorsNamespace())
        continue;

      switch (attributeType.Name)
      {
        case "ActorAttribute":
          SetAttributeOrGenericVersion(attribute, attributeType, ref actorAttribute, ref genericActorAttribute);
          break;
        case "ActorFactoryAttribute":
          SetAttributeOrGenericVersion(attribute, attributeType, ref factoryAttribute, ref genericFactoryAttribute);
          break;
        case "ActorImplementationAttribute":
          SetAttributeOrGenericVersion(attribute, attributeType, ref implementationAttribute, ref genericImplementationAttribute);
          break;
        case "VirtualAttribute":
          virtualAttribute = attribute;
          break;
        case "ActorIdTypeAttribute":
          SetAttributeOrGenericVersion(attribute, attributeType, ref idTypeAttribute, ref genericIdTypeAttribute);
          break;
      }
    }

    if (actorAttribute is not null || genericActorAttribute is not null)
    {
      IReadOnlyDictionary<ITypeSymbol, Location>? baseTypeLocations = CreateBaseTypeLocationDictionary(semanticModel, context.Node, cancellationToken);
      return new ActorData(type, actorAttribute, genericActorAttribute, virtualAttribute, idTypeAttribute, genericIdTypeAttribute, baseTypeLocations);
    }

    if (implementationAttribute is not null || genericImplementationAttribute is not null)
      return new ImplementationData(type, implementationAttribute, genericImplementationAttribute);

    if (factoryAttribute is not null || genericFactoryAttribute is not null)
      return new FactoryData(type, factoryAttribute, genericFactoryAttribute);

    return null;

    static void SetAttributeOrGenericVersion(AttributeData attribute, INamedTypeSymbol attributeType, ref AttributeData? nonGenericAttribute, ref AttributeData? genericAttribute)
    {
      if (attributeType.IsGenericType)
      {
        genericAttribute = attribute;
      }
      else
      {
        nonGenericAttribute = attribute;
      }
    }

    static IReadOnlyDictionary<ITypeSymbol, Location>? CreateBaseTypeLocationDictionary(SemanticModel semanticModel, SyntaxNode node, CancellationToken cancellationToken)
    {
      if (node is not ClassDeclarationSyntax { BaseList.Types: { } baseTypes })
        return null;

      Dictionary<ITypeSymbol, Location> result = new(baseTypes.Count, SymbolEqualityComparer.Default);
      foreach (BaseTypeSyntax baseType in baseTypes)
      {
        TypeInfo typeInfo = semanticModel.GetTypeInfo(baseType.Type, cancellationToken);
        if (typeInfo.Type is not ITypeSymbol type)
          continue;

        result[type] = baseType.GetLocation();
      }

      return result;
    }
  }

  private static DescriptorResult GetDescriptorResult((ImmutableArray<TypeData>, ActorAnalyzerOptions) item, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    (ImmutableArray<TypeData> dataCollection, ActorAnalyzerOptions options) = item;

    if (options == ActorAnalyzerOptions.DisableAll)
      return DescriptorResult.Empty;

    ActorDescriptorFactory factory = new(options, cancellationToken);

    foreach (TypeData data in dataCollection)
    {
      data.AddTo(in factory);
    }

    return factory.GetResult();
  }

  private static ActorAnalyzerOptions GetActorAnalyzerOptions(Compilation compilation, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    return compilation.GetActorAnalyzerOptions();
  }
}
