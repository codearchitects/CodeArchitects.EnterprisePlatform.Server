using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Analyzer;

internal static class DiagnosticDescriptors
{
  public static readonly DiagnosticDescriptor CAESP001 = new DiagnosticDescriptor(
    DiagnosticIds.CAESP001,
    "Experimental feature",
    "This is an experimental feature. It may be changed or removed in future versions.",
    "CodeAnalysis",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAESP002 = new DiagnosticDescriptor(
    DiagnosticIds.CAESP002,
    "Queryable leak",
    "Exposing IQueryable from a repository interface is not recommended. Consider declaring a specific method for each query.",
    "CodeAnalysis",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);
}
