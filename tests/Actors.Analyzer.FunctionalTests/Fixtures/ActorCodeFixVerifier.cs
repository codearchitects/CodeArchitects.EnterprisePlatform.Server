using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace CodeArchitects.Platform.Actors.Analyzer.Fixtures;

internal class ActorCodeFixVerifier
{
  private readonly ActorCodeFixTest _test;

  private ActorCodeFixVerifier(ActorCodeFixTest test)
  {
    _test = test;
  }

  public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) => new(descriptor);

  public static DiagnosticResult Diagnostic(string id, DiagnosticSeverity severity = DiagnosticSeverity.Error) => new(id, severity);

  public ActorCodeFixVerifier AddSource(string source, params DiagnosticResult[] diagnostics)
  {
    AddSource(_test.TestState, source, diagnostics);
    return this;
  }

  public ActorCodeFixVerifier AddSource(string fileName, string source, params DiagnosticResult[] diagnostics)
  {
    AddSource(_test.TestState, source, diagnostics, fileName);
    return this;
  }

  public ActorCodeFixVerifier AddFixedSource(string fixedSource, params DiagnosticResult[] diagnostics)
  {
    AddSource(_test.FixedState, fixedSource, diagnostics);
    return this;
  }

  public ActorCodeFixVerifier AddFixedSource(string fileName, string fixedSource, params DiagnosticResult[] diagnostics)
  {
    AddSource(_test.FixedState, fixedSource, diagnostics, fileName);
    return this;
  }

  public ActorCodeFixVerifier ModifyCompilationOptions(Func<CompilationOptions, CompilationOptions> optionsTransform)
  {
    _test.ModifyCompilationOptions(optionsTransform);
    return this;
  }

  private static void AddSource(SolutionState state, string source, DiagnosticResult[] diagnostics, string? fileName = null)
  {
    string normalizedSource = NormalizeLineEndings(source);

    if (fileName is not null)
    {
      state.Sources.Add((fileName, normalizedSource));
    }
    else
    {
      state.Sources.Add(normalizedSource);
    }

    string path = state.Sources[^1].filename;
    state.ExpectedDiagnostics.AddRange(diagnostics.Select(diagnostic => diagnostic.WithDefaultPath(path)));
  }

  private static string NormalizeLineEndings(string source)
  {
    // Keep a stable newline convention for verifier snapshots across TFMs and OSes.
    return source.Replace("\r\n", "\n").Replace("\r", "\n");
  }

  public static Task Verify(Action<ActorCodeFixVerifier> specification, CancellationToken cancellationToken = default)
  {
    ActorCodeFixTest test = new();
    specification(new ActorCodeFixVerifier(test));

    return test.RunAsync(cancellationToken);
  }
}
