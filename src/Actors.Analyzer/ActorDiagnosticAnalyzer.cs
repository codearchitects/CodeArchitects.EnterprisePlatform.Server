using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class ActorDiagnosticAnalyzer : DiagnosticAnalyzer
{
  private static readonly Dictionary<string, DiagnosticDescriptor> s_idToDescriptors = DiagnosticDescriptors.All.ToDictionary(descriptor => descriptor.Id);

  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.All);

  public override void Initialize(AnalysisContext context)
  {
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
    context.EnableConcurrentExecution();

    context.RegisterCompilationStartAction(ReportSerializedDiagnostics);
  }

  private static void ReportSerializedDiagnostics(CompilationStartAnalysisContext context)
  {
    if (context.Compilation.GetActorAnalyzerOptions().ShouldDisableActorDiagnostics())
      return;

    if (!TryGetActorDiagnosticSyntaxTree(context.Compilation.SyntaxTrees, out SyntaxTree? diagnosticSyntaxTree))
      return;

    Dictionary<string, List<DiagnosticReference>> diagnostics = new();

    foreach (DiagnosticReference reference in GetReferences(diagnosticSyntaxTree, context.CancellationToken))
    {
      if (!diagnostics.TryGetValue(reference.FilePath, out List<DiagnosticReference>? references))
      {
        references = new();
        diagnostics.Add(reference.FilePath, references);
      }

      references.Add(reference);
    }

    context.RegisterSyntaxTreeAction(context => AnalyzeSyntaxTree(context, diagnostics));

    static bool TryGetActorDiagnosticSyntaxTree(IEnumerable<SyntaxTree> syntaxTrees, [NotNullWhen(true)] out SyntaxTree? diagnosticSyntaxTree)
    {
      foreach (SyntaxTree syntaxTree in syntaxTrees)
      {
        if (syntaxTree.FilePath.EndsWith("ActorDiagnostics.g.cs"))
        {
          diagnosticSyntaxTree = syntaxTree;
          return true;
        }
      }

      diagnosticSyntaxTree = null;
      return false;
    }

    static IEnumerable<DiagnosticReference> GetReferences(SyntaxTree diagnosticsSyntaxTree, CancellationToken cancellationToken)
    {
      SourceText text = diagnosticsSyntaxTree.GetText(cancellationToken);

      bool foundStart = false;
      foreach (TextLine textLine in text.Lines)
      {
        string line = textLine.ToString();
        if (!foundStart)
        {
          foundStart |= line is "#if false";
          continue;
        }

        if (!foundStart)
          continue;
        
        if (line is "#endif")
          yield break;

        yield return DiagnosticReference.Parse(line);
      }
    }
  }

  private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context, Dictionary<string, List<DiagnosticReference>> diagnostics)
  {
    SyntaxTree syntaxTree = context.Tree;
    if (diagnostics.TryGetValue(syntaxTree.FilePath, out List<DiagnosticReference>? references) && references != null)
    {
      foreach (DiagnosticReference reference in references)
      {
        DiagnosticDescriptor descriptor = s_idToDescriptors[reference.DiagnosticId];
        Location location = reference.GetLocation(syntaxTree);
        object[] args = reference.Args.ToArray();
        ImmutableDictionary<string, string?>? properties = reference.GetPropertyDictionary();
        
        Diagnostic diagnostic = reference.Severity.HasValue
          ? Diagnostic.Create(
              descriptor: descriptor,
              location: location,
              effectiveSeverity: reference.Severity.Value,
              additionalLocations: null,
              properties: properties,
              messageArgs: args)
          : Diagnostic.Create(
              descriptor: descriptor,
              location: location,
              properties: properties,
              messageArgs: args);
        context.ReportDiagnostic(diagnostic);
      }
    }
  }
}
