using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace CodeArchitects.Platform.Actors.Analyzer.Fixtures;

internal class ActorAnalyzerTest : CSharpAnalyzerTest<ActorDiagnosticAnalyzer, XUnitVerifier>
{
  public ActorAnalyzerTest()
  {
    ActorTest.SetupTestState(TestState);
  }

  protected override string DefaultTestProjectName => ActorTest.DefaultTestProjectName;

  protected override ParseOptions CreateParseOptions() => ActorTest.CreateParseOptions();

  protected override async Task<Compilation> GetProjectCompilationAsync(Project project, IVerifier verifier, CancellationToken cancellationToken)
  {
    Compilation originalCompilation = await base.GetProjectCompilationAsync(project, verifier, cancellationToken);

    return ActorTest.GetProjectCompilation(originalCompilation, cancellationToken);
  }
}
