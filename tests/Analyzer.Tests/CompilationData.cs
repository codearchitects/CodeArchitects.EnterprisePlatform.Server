using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CodeArchitects.Platform.Analyzer.Tests;

public record CompilationData(
  AdhocWorkspace Workspace,
  Solution Solution,
  Project Project,
  Document Document,
  ImmutableArray<Diagnostic> Diagnostics);
