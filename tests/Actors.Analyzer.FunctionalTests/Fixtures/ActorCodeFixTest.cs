using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace CodeArchitects.Platform.Actors.Analyzer.Fixtures;

internal class ActorCodeFixTest : CSharpCodeFixTest<ActorDiagnosticAnalyzer, ActorCodeFixProvider, XUnitVerifier>
{
  private Func<CompilationOptions, CompilationOptions>? _optionsTransform;

  public ActorCodeFixTest()
  {
    ActorTest.SetupTestState(TestState);

    const string editorConfig = """
      root = true

      [*]
      end_of_line = lf
      """;

    TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));
    FixedState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig));

    SolutionTransforms.Add((solution, _) => NormalizeSolutionLineEndings(solution));

    SolutionTransforms.Add((solution, projectId) =>
      solution.WithOptions(solution.Options.WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, "\n")));
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

  private static Solution NormalizeSolutionLineEndings(Solution solution)
  {
    foreach (DocumentId documentId in solution.Projects.SelectMany(project => project.DocumentIds))
    {
      Document? document = solution.GetDocument(documentId);
      if (document is null)
      {
        continue;
      }

      SourceText? sourceText = document.GetTextAsync().GetAwaiter().GetResult();
      string normalized = sourceText.ToString().Replace("\r\n", "\n").Replace("\r", "\n");
      if (normalized == sourceText.ToString())
      {
        continue;
      }

      solution = solution.WithDocumentText(documentId, SourceText.From(normalized, sourceText.Encoding));
    }

    return solution;
  }
}