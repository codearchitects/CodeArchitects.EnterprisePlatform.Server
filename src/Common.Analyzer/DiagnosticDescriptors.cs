using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Common.Analyzer;

internal static class DiagnosticDescriptors
{
  public static readonly DiagnosticDescriptor CAEP001 = new DiagnosticDescriptor(
    DiagnosticIds.CAEP001,
    "Experimental feature",
    "This is an experimental feature. It may be changed or removed in future versions.",
    "CodeAnalysis",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);
}
