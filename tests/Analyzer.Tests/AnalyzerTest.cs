using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Analyzer.Tests
{
  public abstract class AnalyzerTest : CompilationTest
  {
    protected Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string code)
      => GetCompilationDataAsync(code)
        .ContinueWith(x => x.Result.Diagnostics);
  }
}
