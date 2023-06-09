using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace CodeArchitects.Platform.Actors.Analyzer.Fixtures;

internal class ActorCodeFixTest : CSharpCodeFixTest<ActorDiagnosticAnalyzer, ActorCodeFixProvider, XUnitVerifier>
{
  private Func<CompilationOptions, CompilationOptions>? _optionsTransform;

  public ActorCodeFixTest()
  {
    ActorTest.SetupTestState(TestState);
  }

  public void ModifyCompilationOptions(Func<CompilationOptions, CompilationOptions> optionsTransform)
  {
    _optionsTransform = optionsTransform;
  }

  protected override string DefaultTestProjectName => ActorTest.DefaultTestProjectName;

  protected override ParseOptions CreateParseOptions() => ActorTest.CreateParseOptions();

  protected override async Task<Compilation> GetProjectCompilationAsync(Project project, IVerifier verifier, CancellationToken cancellationToken)
  {
    Compilation originalCompilation = await base.GetProjectCompilationAsync(project, verifier, cancellationToken);
    if (_optionsTransform is not null)
    {
      originalCompilation = originalCompilation.WithOptions(_optionsTransform(originalCompilation.Options));
    }
    
    return ActorTest.GetProjectCompilation(originalCompilation, cancellationToken);
  }
}