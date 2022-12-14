using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Reflection;

namespace CodeArchitects.Platform.Analyzer.Tests;

public abstract class CompilationTest
{
  protected virtual IEnumerable<Type> ReferencedAssemblyMarkers => Enumerable.Empty<Type>();

  protected virtual IEnumerable<Assembly> ReferencedAsseblies => Enumerable.Empty<Assembly>();

  protected abstract Type AnalyzerType { get; }

  protected async Task<CompilationData> GetCompilationDataAsync(string code)
  {
    AdhocWorkspace workspace = new();
    Solution solution = workspace.CurrentSolution;
    ProjectId projectId = ProjectId.CreateNewId();
    DocumentId documentId = DocumentId.CreateNewId(projectId);

    solution = solution
      .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
      .WithProjectOutputRefFilePath(projectId, "test/outputRefFilePath")
      .WithProjectOutputFilePath(projectId, "test/outputFilePath")
      .AddDocument(documentId, "Program.cs", code);

    Project project = solution.GetProject(projectId)!;

    project = project.AddMetadataReferences(GetAllReferences(typeof(object).Assembly));

    foreach (Type type in ReferencedAssemblyMarkers)
    {
      project = project.AddMetadataReferences(GetAllReferences(type.Assembly));
    }

    foreach (Assembly assembly in ReferencedAsseblies)
    {
      project = project.AddMetadataReferences(GetAllReferences(assembly));
    }

    if (!workspace.TryApplyChanges(project.Solution))
    {
      throw new Exception("Could not apply changes to the workspace");
    }

    Compilation? compilation = await project.GetCompilationAsync();

    if (compilation is null)
    {
      throw new Exception("Could not get compilation.");
    }

    CompilationWithAnalyzers compilationWithAnalyzer = compilation
      .WithAnalyzers(ImmutableArray.Create((DiagnosticAnalyzer)Activator.CreateInstance(AnalyzerType)!));

    return new CompilationData(
      workspace,
      workspace.CurrentSolution,
      workspace.CurrentSolution.GetProject(projectId)!,
      workspace.CurrentSolution.GetDocument(documentId)!,
      await compilationWithAnalyzer.GetAllDiagnosticsAsync());
  }

  private static MetadataReference[] GetAllReferences(Assembly assembly)
  {
    return assembly.GetReferencedAssemblies()
      .Select(x => Assembly.Load(x.FullName))
      .Append(assembly)
      .Select(x => MetadataReference.CreateFromFile(x.Location)).Cast<MetadataReference>().ToArray();
  }
}
