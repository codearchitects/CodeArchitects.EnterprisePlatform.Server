using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CodeArchitects.Platform.Analyzer.Tests;

public abstract class AnalyzerTest : CompilationTest
{
  protected async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string code)
  {
    CompilationData compilationData = await GetCompilationDataAsync(code);
    return compilationData.Diagnostics;
  }
}
