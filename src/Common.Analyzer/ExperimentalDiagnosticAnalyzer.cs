using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CodeArchitects.Platform.Common.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ExperimentalDiagnosticAnalyzer : DiagnosticAnalyzer
{
  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
    DiagnosticDescriptors.CAEP000);

  private static readonly string s_experimentalAttributeName = "CodeArchitects.Platform.Common.CodeAnalysis.ExperimentalAttribute";

  public override void Initialize(AnalysisContext context)
  {
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
    context.EnableConcurrentExecution();

    context.RegisterSyntaxNodeAction(AnalyzeIdentifierNameNode, SyntaxKind.IdentifierName);
    context.RegisterSyntaxNodeAction(AnalyzeGenericNameNode, SyntaxKind.GenericName);
    context.RegisterSyntaxNodeAction(AnalyzeObjectCreationNode, SyntaxKind.ObjectCreationExpression);
  }

  private void AnalyzeIdentifierNameNode(SyntaxNodeAnalysisContext context)
  {
    context.CancellationToken.ThrowIfCancellationRequested();

    if (context.Node is not IdentifierNameSyntax node)
    {
      Debug.Fail($"Invalid syntax kind registered for this action. Expected {nameof(IdentifierNameSyntax)} but got {context.Node.GetType().Name} instead.");
      return;
    }

    bool isExperimental = context.SemanticModel
      .GetSymbolInfo(node)
      .Symbol?
      .GetAttributes()
      .Any(x => x.AttributeClass?.ToString() == s_experimentalAttributeName)
      ?? false;

    if (isExperimental && (node.Parent is not ObjectCreationExpressionSyntax || node.Parent.Parent is not EqualsValueClauseSyntax))
    {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CAEP000, node.GetLocation()));
    }
  }

  private void AnalyzeGenericNameNode(SyntaxNodeAnalysisContext context)
  {
    context.CancellationToken.ThrowIfCancellationRequested();

    if (context.Node is not GenericNameSyntax node)
    {
      Debug.Fail($"Invalid syntax kind registered for this action. Expected {nameof(GenericNameSyntax)} but got {context.Node.GetType().Name} instead.");
      return;
    }

    bool isExperimental = context.SemanticModel
      .GetSymbolInfo(node)
      .Symbol?
      .GetAttributes()
      .Any(x => x.AttributeClass?.ToString() == s_experimentalAttributeName)
      ?? false;

    if (isExperimental && (node.Parent is not ObjectCreationExpressionSyntax || node.Parent.Parent is not EqualsValueClauseSyntax))
    {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CAEP000, node.GetLocation()));
    }
  }

  private void AnalyzeObjectCreationNode(SyntaxNodeAnalysisContext context)
  {
    context.CancellationToken.ThrowIfCancellationRequested();

    if (context.Node is not ObjectCreationExpressionSyntax node)
    {
      Debug.Fail($"Invalid syntax kind registered for this action. Expected {nameof(ObjectCreationExpressionSyntax)} but got {context.Node.GetType().Name} instead.");
      return;
    }

    bool isExperimental = context.SemanticModel
      .GetSymbolInfo(node)
      .Symbol?
      .GetAttributes()
      .Any(x => x.AttributeClass?.ToString() == s_experimentalAttributeName)
      ?? false;

    if (isExperimental)
    {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CAEP000, node.GetLocation()));
    }
  }
}
