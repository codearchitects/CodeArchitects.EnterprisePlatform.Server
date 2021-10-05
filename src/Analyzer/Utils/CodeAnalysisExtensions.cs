using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeArchitects.Platform.Analyzer.Utils
{
  internal static class CodeAnalysisExtensions
  {
    public static string? GetFullName(this ExpressionSyntax expression, SemanticModel semanticModel)
    {
      return semanticModel.GetSymbolInfo(expression).Symbol?.ToString();
    }
  }
}
