using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace CodeArchitects.Platform.Analyzer.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ExperimentalAnalyzer : DiagnosticAnalyzer
{
  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
    DiagnosticDescriptors.CAESP001);

  private static readonly string s_experimentalAttributeName = "CodeArchitects.Platform.CodeAnalysis.ExperimentalAttribute";

  public override void Initialize(AnalysisContext context)
  {
    context.EnableConcurrentExecution();
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

    context.RegisterSyntaxNodeAction(AnalyzeIdentifierNameNode, SyntaxKind.IdentifierName);
    context.RegisterSyntaxNodeAction(AnalyzeGenericNameNode, SyntaxKind.GenericName);
    context.RegisterSyntaxNodeAction(AnalyzeObjectCreationNode, SyntaxKind.ObjectCreationExpression);
  }

  private void AnalyzeIdentifierNameNode(SyntaxNodeAnalysisContext context)
  {
    if (context.Node is not IdentifierNameSyntax node)
    {
      throw new Exception($"Invalid syntax kind registered for this action. Expected {nameof(IdentifierNameSyntax)} but got {context.Node.GetType().Name} instead.");
    }

    bool isExperimental = context.SemanticModel
      .GetSymbolInfo(node)
      .Symbol?
      .GetAttributes()
      .Any(x => x.AttributeClass?.ToString() == s_experimentalAttributeName)
      ?? false;

    if (isExperimental && (node.Parent is not ObjectCreationExpressionSyntax || node.Parent.Parent is not EqualsValueClauseSyntax))
    {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CAESP001, node.GetLocation()));
    }
  }
  private void AnalyzeGenericNameNode(SyntaxNodeAnalysisContext context)
  {
    if (context.Node is not GenericNameSyntax node)
    {
      throw new Exception($"Invalid syntax kind registered for this action. Expected {nameof(GenericNameSyntax)} but got {context.Node.GetType().Name} instead.");
    }

    bool isExperimental = context.SemanticModel
      .GetSymbolInfo(node)
      .Symbol?
      .GetAttributes()
      .Any(x => x.AttributeClass?.ToString() == s_experimentalAttributeName)
      ?? false;

    if (isExperimental && (node.Parent is not ObjectCreationExpressionSyntax || node.Parent.Parent is not EqualsValueClauseSyntax))
    {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CAESP001, node.GetLocation()));
    }
  }

  private void AnalyzeObjectCreationNode(SyntaxNodeAnalysisContext context)
  {
    if (context.Node is not ObjectCreationExpressionSyntax node)
    {
      throw new Exception($"Invalid syntax kind registered for this action. Expected {nameof(ObjectCreationExpressionSyntax)} but got {context.Node.GetType().Name} instead.");
    }

    bool isExperimental = context.SemanticModel
      .GetSymbolInfo(node)
      .Symbol?
      .GetAttributes()
      .Any(x => x.AttributeClass?.ToString() == s_experimentalAttributeName)
      ?? false;

    if (isExperimental)
    {
      context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CAESP001, node.GetLocation()));
    }
  }
}
