using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System.Reflection;

namespace CodeArchitects.Platform.Common.Analyzer.Fixtures;

public class CommonAnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
  where TAnalyzer : DiagnosticAnalyzer, new()
{
  private static readonly string s_commonAssemblyDirectory = Directory.GetCurrentDirectory()
    .Replace("tests", "src")
    .Replace("Common.Analyzer.FunctionalTests", "Common")
    .Replace("net7.0", "netstandard2.1");

  private static readonly string s_commonAssemblyLocation = Path.Combine(s_commonAssemblyDirectory, "CodeArchitects.Platform.Common.dll");

  public CommonAnalyzerTest()
  {
    TestState.ReferenceAssemblies = ReferenceAssemblies.NetStandard.NetStandard21;
    TestState.AdditionalReferences.Add(Assembly.LoadFrom(s_commonAssemblyLocation));
  }

  protected override string DefaultTestProjectName => "CodeArchitects.Platform.Common.Analyzer.FunctionalTests";

  protected override ParseOptions CreateParseOptions()
  {
    return new CSharpParseOptions(languageVersion: LanguageVersion.Preview);
  }
}
