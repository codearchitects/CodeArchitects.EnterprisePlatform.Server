using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Analyzer.Fixtures;

internal static class ActorTest
{
  private static readonly string s_actorsAssemblyDirectory = Directory.GetCurrentDirectory()
    .Replace("tests", "src")
    .Replace("Actors.Analyzer.FunctionalTests", "Actors")
    .Replace("net7.0", "netstandard2.1");

  private static readonly string s_actorsAssemblyLocation = Path.Combine(s_actorsAssemblyDirectory, "CodeArchitects.Platform.Actors.dll");

  private static readonly string s_messagingAssemblyLocation = s_actorsAssemblyLocation.Replace("Actors", "Messaging");

  public static void SetupTestState(SolutionState testState)
  {
    testState.ReferenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard21;
    testState.AdditionalReferences.Add(Assembly.LoadFrom(s_actorsAssemblyLocation));
    testState.AdditionalReferences.Add(Assembly.LoadFrom(s_messagingAssemblyLocation));
  }

  public const string DefaultTestProjectName = "CodeArchitects.Platform.Actors.Analyzer.FunctionalTests";

  public static CSharpParseOptions CreateParseOptions()
  {
    return new CSharpParseOptions(languageVersion: LanguageVersion.Preview);
  }

  public static Compilation GetProjectCompilation(Compilation originalCompilation, CancellationToken cancellationToken)
  {
    GeneratorDriver driver = CSharpGeneratorDriver.Create(
      generators: new[] { new ActorSourceGenerator().AsSourceGenerator() },
      parseOptions: CreateParseOptions());
    driver.RunGeneratorsAndUpdateCompilation(originalCompilation, out Compilation compilation, out _, cancellationToken);

    return compilation;
  }
}
