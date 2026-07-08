using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace CodeArchitects.Platform.Actors.Analyzer.Fixtures;

internal class ActorAnalyzerVerifier
{
  private readonly ActorAnalyzerTest _test;

  private ActorAnalyzerVerifier(ActorAnalyzerTest test)
  {
    _test = test;
  }

  public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) => new(descriptor);

  public static DiagnosticResult Diagnostic(string id, DiagnosticSeverity severity = DiagnosticSeverity.Error) => new(id, severity);

  public ActorAnalyzerVerifier AddSource(string source, params DiagnosticResult[] diagnostics)
  {
    SourceFileList sources = _test.TestState.Sources;
    sources.Add(source);
    string path = sources[^1].filename;

    _test.TestState.ExpectedDiagnostics.AddRange(diagnostics.Select(diagnostic => diagnostic.WithDefaultPath(path)));
    return this;
  }

  public ActorAnalyzerVerifier ModifyCompilationOptions(Func<CompilationOptions, CompilationOptions> optionsTransform)
  {
    _test.ModifyCompilationOptions(optionsTransform);
    return this;
  }

  public static Task Verify(Action<ActorAnalyzerVerifier> specification, CancellationToken cancellationToken = default)
  {
    ActorAnalyzerTest test = new();
    specification(new ActorAnalyzerVerifier(test));

    return test.RunAsync(cancellationToken);
  }
}
