using CodeArchitects.Platform.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeArchitects.Platform.Analyzer.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RepositoryAnalyzer : DiagnosticAnalyzer
{
  private readonly static Regex s_iRepositoryRegex = new Regex(@"CodeArchitects\.Platform\.Data\.IRepository<[^<>,]+, [^<>,]+>", RegexOptions.Compiled, TimeSpan.FromMilliseconds(500));
  private readonly static Regex s_iQueryableRegex = new Regex(@"System\.Linq\.IQueryable<[^<>,]+>", RegexOptions.Compiled, TimeSpan.FromMilliseconds(500));

  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
    DiagnosticDescriptors.CAESP002);

  public override void Initialize(AnalysisContext context)
  {
    context.EnableConcurrentExecution();
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

    context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclarationNode, SyntaxKind.InterfaceDeclaration);
  }

  private void AnalyzeInterfaceDeclarationNode(SyntaxNodeAnalysisContext context)
  {
    if (context.Node is not InterfaceDeclarationSyntax node)
    {
      throw new Exception($"Invalid syntax kind registered for this action. Expected {nameof(InterfaceDeclarationSyntax)} but got {context.Node.GetType().Name} instead.");
    }

    if (node.BaseList is null)
      return;

    if (node.BaseList is null || !node.BaseList.Types.Any(x => s_iRepositoryRegex.IsMatch(x.Type.GetFullName(context.SemanticModel))))
      return;

    INamedTypeSymbol? interfaceSymbol = context.SemanticModel.GetDeclaredSymbol(node);
    if (interfaceSymbol is null)
      return;

    foreach (ISymbol symbol in interfaceSymbol.GetMembers().Where(x => x.Kind is SymbolKind.Method or SymbolKind.Property))
    {
      ITypeSymbol? type = symbol switch
      {
        IMethodSymbol methodSymbol when methodSymbol.MethodKind is
          not MethodKind.PropertyGet and
          not MethodKind.PropertySet => methodSymbol.ReturnType,
        IPropertySymbol propertySymbol => propertySymbol.Type,
        _ => null
      };

      if (type is null)
        continue;

      if (s_iQueryableRegex.IsMatch(type.ToString()))
      {
        Location? location = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().GetLocation();
        if (location is not null)
        {
          context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.CAESP002, location));
        }
      }
    }
  }
}
