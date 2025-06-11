using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace CodeArchitects.Platform.Actors.Analyzer.CodeFixes;

internal abstract class FixingActionProvider
{
  private static readonly Dictionary<string, FixingActionProvider?> s_providers =
      typeof(FixingActionProvider).Assembly.DefinedTypes
      .Where(type => typeof(FixingActionProvider).IsAssignableFrom(type) && !type.IsAbstract)
      .Select(type => Activator.CreateInstance(type) as FixingActionProvider)
      .Where(provider => provider != null)
      .ToDictionary(provider => provider!.DiagnosticId);

  public static readonly string[] FixableDiagnosticIds = s_providers.Keys.ToArray();

  protected abstract string DiagnosticId { get; }

  protected static ValueTask<CodeAction?> None => new(null as CodeAction);

  protected abstract ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, SyntaxNode node, IReadOnlyList<string> properties, CancellationToken cancellationToken);

  public static ValueTask<CodeAction?> GetFixingActionAsync(Document document, SyntaxNode root, Diagnostic diagnostic, CancellationToken cancellationToken)
  {
    if (!s_providers.TryGetValue(diagnostic.Id, out FixingActionProvider? provider) || provider is null)
      return None;

    SyntaxNode? node = root.FindNode(diagnostic.Location.SourceSpan);
    if (node is null)
      return None;

    IReadOnlyList<string> properties;

    if (diagnostic.Properties is null)
    {
      properties = Array.Empty<string>();
    }
    else
    {
      properties = diagnostic.Properties
          .OrderBy(pair => pair.Key)
          .Select(pair => pair.Value)
          .ToList()!;
    }

    return provider.GetFixingActionAsync(document, root, node, properties, cancellationToken);
  }

  protected static ValueTask<CodeAction?> Fail(string message)
  {
    Debug.Fail(message);
    return None;
  }

  protected static ClassDeclarationSyntax AddToBaseList(ClassDeclarationSyntax @class, TypeSyntax type)
  {
    BaseTypeSyntax baseType = SyntaxFactory.SimpleBaseType(type);

    if (@class.BaseList is BaseListSyntax baseList)
    {
      return @class.WithBaseList(baseList.AddTypes(baseType));
    }

    baseList = SyntaxFactory.BaseList(
      colonToken: SyntaxFactory.Token(
        leading: new SyntaxTriviaList(SyntaxFactory.Space),
        kind: SyntaxKind.ColonToken,
        trailing: new SyntaxTriviaList(SyntaxFactory.Space)),
      types: SyntaxFactory.SingletonSeparatedList(baseType));

    return @class
      .WithIdentifier(@class.Identifier.WithTrailingTrivia(SyntaxTriviaList.Empty))
      .WithBaseList(baseList.WithTrailingTrivia(@class.Identifier.TrailingTrivia));
  }

  protected static IMethodSymbol? GetInterfaceMethodSymbol(IMethodSymbol methodSymbol)
  {
    INamedTypeSymbol actorType = methodSymbol.ContainingType;
    foreach (INamedTypeSymbol interfaceSymbol in actorType.AllInterfaces)
    {
      foreach (ISymbol member in interfaceSymbol.GetMembers())
      {
        if (member.Kind is not SymbolKind.Method)
          continue;

        if (SymbolEqualityComparer.Default.Equals(actorType.FindImplementationForInterfaceMember(member), methodSymbol))
          return (IMethodSymbol)member;
      }
    }

    return null;
  }
}
