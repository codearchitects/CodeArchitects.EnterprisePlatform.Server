using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Utils;

internal static class Format
{
  public static readonly SymbolDisplayFormat GlobalFullName = new(
    globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
    genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

  public static readonly SymbolDisplayFormat FullName = new(
    globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
    genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

  public static readonly SymbolDisplayFormat Name = new(
    globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
    genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
}
