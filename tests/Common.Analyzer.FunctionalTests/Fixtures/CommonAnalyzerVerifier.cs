using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace CodeArchitects.Platform.Common.Analyzer.Fixtures;

internal class CommonAnalyzerVerifier<TAnalyzer> : AnalyzerVerifier<TAnalyzer, CommonAnalyzerTest<TAnalyzer>, XUnitVerifier>
  where TAnalyzer : DiagnosticAnalyzer, new()
{
}
