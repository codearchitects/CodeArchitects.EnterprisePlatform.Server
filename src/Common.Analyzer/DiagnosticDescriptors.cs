#pragma warning disable RS2008 // Enable analyzer release tracking

using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Common.Analyzer;

internal static class DiagnosticDescriptors
{
  public static readonly DiagnosticDescriptor CAEP000 = new DiagnosticDescriptor(
    DiagnosticIds.CAEP000,
    "Experimental feature",
    "This is an experimental feature. It may be changed or removed in future versions.",
    "CodeAnalysis",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);
}
