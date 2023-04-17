using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeArchitects.Platform.Actors.Analyzer.Utils;

internal static class SyntaxNodeExtensions
{
  public static BaseNamespaceDeclarationSyntax? GetNamespaceDeclaration(this SyntaxNode node)
  {
    while (!node.IsKind(SyntaxKind.CompilationUnit))
    {
      if (node is BaseNamespaceDeclarationSyntax namespaceDeclaration)
        return namespaceDeclaration;

      node = node.Parent!;
    }

    return null;
  }

  public static TypeSyntax? GetTarget(this AttributeSyntax attributeSyntax, NameSyntax nameSyntax)
  {
    switch (nameSyntax)
    {
      case IdentifierNameSyntax:
        if (attributeSyntax.ArgumentList is not AttributeArgumentListSyntax argumentListSyntax)
          goto default;

        if (argumentListSyntax.Arguments is not [AttributeArgumentSyntax argumentSyntax])
          goto default;

        if (argumentSyntax.NameEquals is not null && argumentSyntax.NameEquals.Name.Identifier.ValueText is not "InterfaceType")
          goto default;

        if (argumentSyntax.Expression is not TypeOfExpressionSyntax typeOfExpressionSyntax)
          goto default;

        return typeOfExpressionSyntax.Type;

      case GenericNameSyntax genericNameSyntax:
        if (genericNameSyntax.TypeArgumentList.Arguments is not [TypeSyntax typeArgument])
          goto default;

        return typeArgument;

      case QualifiedNameSyntax qualifiedNameSyntax:
        return attributeSyntax.GetTarget(qualifiedNameSyntax.Right);

      default:
        return null;
    }
  }
}
