using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Utils;

internal static class CompilationExtensions
{
  public static ActorAnalyzerOptions GetActorAnalyzerOptions(this Compilation compilation)
  {
    ActorAnalyzerOptions options = default;

    foreach (AttributeData attribute in compilation.Assembly.GetAttributes())
    {
      INamedTypeSymbol? attributeType = attribute.AttributeClass;
      if (attributeType is null)
        continue;

      if (!attributeType.ContainingNamespace.IsCodeArchitectsPlatformActorsCodeAnalysisNamespace())
        continue;

      if (attributeType.Name == "DisableActorDiagnosticsAttribute")
      {
        options |= ActorAnalyzerOptions.DisableActorDiagnostics;
        continue;
      }

      if (attributeType.Name == "DisableActorFactoryGenerationAttribute")
      {
        options |= ActorAnalyzerOptions.DisableActorFactoryGeneration;
      }
    }

    return options;
  }
}
