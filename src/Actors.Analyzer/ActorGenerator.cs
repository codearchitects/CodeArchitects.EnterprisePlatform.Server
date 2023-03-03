using CodeArchitects.Platform.Actors.Analyzer.Descriptors;
using CodeArchitects.Platform.Actors.Analyzer.Renderers;
using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace CodeArchitects.Platform.Actors.Analyzer;

[Generator]
internal class ActorGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    var optionProvider = context.CompilationProvider.Select(GetAnalyzerOptions);

    var values = context.SyntaxProvider
      .CreateSyntaxProvider(OfTypesWithAttributes, GetTypeData)
      .Where(static data => data is not null)
      .Collect()
      .Combine(optionProvider)
      .Select(GetDescriptorResults);

    context.RegisterSourceOutput(values, Execute);
  }

  private static void Execute(SourceProductionContext context, DiagnosticResult<RecordList<ActorDescriptor>> result)
  {
    ActorFactoryRenderer factoryRenderer = new(context);

    foreach (ActorDescriptor descriptor in result.Value)
    {
      factoryRenderer.Render(descriptor);
    }

    if (result.Diagnostics.Count == 0)
      return;

    foreach (Diagnostic diagnostic in result.Diagnostics)
    {
      context.ReportDiagnostic(diagnostic);
    }
  }

  private static bool OfTypesWithAttributes(SyntaxNode node, CancellationToken cancellationToken)
  {
    return node is
      ClassDeclarationSyntax { AttributeLists.Count: > 0 } or
      InterfaceDeclarationSyntax { AttributeLists.Count: > 0 };
  }

  private static TypeData? GetTypeData(GeneratorSyntaxContext context, CancellationToken cancellationToken)
  {
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
          if (attributeType.IsGenericType)
          {
            genericActorAttribute = attribute;
          }
          else
          {
            actorAttribute = attribute;
          }
          break;
        case "ActorFactoryAttribute":
          if (attributeType.IsGenericType)
          {
            genericFactoryAttribute = attribute;
          }
          else
          {
            factoryAttribute = attribute;
          }
          break;
        case "ActorImplementationAttribute":
          if (attributeType.IsGenericType)
          {
            genericImplementationAttribute = attribute;
          }
          else
          {
            implementationAttribute = attribute;
          }
          break;
        case "VirtualAttribute":
          virtualAttribute = attribute;
          break;
        case "ActorIdTypeAttribute":
          if (attributeType.IsGenericType)
          {
            genericIdTypeAttribute = attribute;
          }
          else
          {
            idTypeAttribute = attribute;
          }
          break;
      }
    }

    return
      actorAttribute is not null          || genericActorAttribute is not null          ? new ActorData(type, actorAttribute, genericActorAttribute, virtualAttribute, idTypeAttribute, genericIdTypeAttribute) :
      factoryAttribute is not null        || genericFactoryAttribute is not null        ? new FactoryData(type, factoryAttribute, genericFactoryAttribute) :
      implementationAttribute is not null || genericImplementationAttribute is not null ? new ImplementationData(type, implementationAttribute, genericImplementationAttribute) :
      null;
  }

  private static DiagnosticResult<RecordList<ActorDescriptor>> GetDescriptorResults((ImmutableArray<TypeData?>, ActorAnalyzerOptions) item, CancellationToken cancellationToken)
  {
    (ImmutableArray<TypeData?> dataCollection, ActorAnalyzerOptions options) = item;
    ActorDescriptorFactory factory = new(options);

    foreach (TypeData? data in dataCollection)
    {
      switch (data)
      {
        case FactoryData factoryData:
          factory.AddFactoryData(factoryData);
          break;
        case ImplementationData implementationData:
          factory.AddImplementationData(implementationData);
          break;
      }
    }

    foreach (TypeData? data in dataCollection)
    {
      if (data is not ActorData actorData)
        continue;

      factory.AddActorData(actorData);
    }

    return factory.GetResult();
  }

  private static ActorAnalyzerOptions GetAnalyzerOptions(Compilation compilation, CancellationToken cancellationToken)
  {
    ActorAnalyzerOptions options = ActorAnalyzerOptions.None;

    foreach (AttributeData attribute in compilation.Assembly.GetAttributes())
    {
      if (attribute.IsDisableActorDiagnosticAttribute())
      {
        options |= ActorAnalyzerOptions.DisableDiagnostics;
      }
      else if (attribute.IsDisableActorFactoryGenerationAttribute())
      {
        options |= ActorAnalyzerOptions.DisableFactoryGeneration;
      }
    }

    return options;
  }
}
