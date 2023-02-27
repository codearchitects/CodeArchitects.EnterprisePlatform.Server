using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Utils;

internal static class SymbolExtensions
{
  public static bool IsDisableActorDiagnosticAttribute(this AttributeData attribute)
  {
    INamedTypeSymbol? attributeType = attribute.AttributeClass;
    if (attributeType is null)
      return false;

    if (attributeType.Name != "DisableActorDiagnosticsAttribute")
      return false;

    return IsCodeArchitectsPlatformActorsCodeAnalysisNamespace(attributeType.ContainingNamespace);
  }

  public static bool IsDisableActorFactoryGenerationAttribute(this AttributeData attribute)
  {
    INamedTypeSymbol? attributeType = attribute.AttributeClass;
    if (attributeType is null)
      return false;

    if (attributeType.Name != "DisableActorFactoryGenerationAttribute")
      return false;

    return IsCodeArchitectsPlatformActorsCodeAnalysisNamespace(attributeType.ContainingNamespace);
  }

  public static bool IsVirtualAttribute(this AttributeData attribute)
  {
    INamedTypeSymbol? attributeType = attribute.AttributeClass;
    if (attributeType is null)
      return false;

    if (attributeType.Name != "VirtualAttribute")
      return false;

    return IsCodeArchitectsPlatformActorsNamespace(attributeType.ContainingNamespace);
  }

  public static bool IsActorConstructorAttribute(this AttributeData attribute)
  {
    INamedTypeSymbol? attributeType = attribute.AttributeClass;
    if (attributeType is null)
      return false;

    if (attributeType.Name != "ActorConstructorAttribute")
      return false;

    return IsCodeArchitectsPlatformActorsNamespace(attributeType.ContainingNamespace);
  }

  public static bool IsActorIdSourceType(this ITypeSymbol type, [NotNullWhen(true)] out ITypeSymbol? idType)
  {
    if (type is not INamedTypeSymbol namedType)
    {
      idType = null;
      return false;
    }
  
    if (!namedType.IsGenericType)
    {
      idType = null;
      return false;
    }
  
    if (namedType.Name != "IActorIdSource")
    {
      idType = null;
      return false;
    }
  
    if (!namedType.ContainingNamespace.IsCodeArchitectsPlatformActorsNamespace())
    {
      idType = null;
      return false;
    }
  
    idType = namedType.TypeArguments[0];
    return true;
  }

  public static bool IsGenericTaskType(this ITypeSymbol type, [NotNullWhen(true)] out ITypeSymbol? resultType)
  {
    if (type is not INamedTypeSymbol namedType)
    {
      resultType = null;
      return false;
    }

    if (!namedType.IsGenericType)
    {
      resultType = null;
      return false;
    }

    if (namedType.Name != "Task")
    {
      resultType = null;
      return false;
    }

    if (!IsSystemThreadingTasksNamespace(namedType.ContainingNamespace))
    {
      resultType = null;
      return false;
    }

    resultType = namedType.TypeArguments[0];
    return true;
  }

  public static bool IsTaskLikeType(this ITypeSymbol type)
  {
    if (type.Name is not "Task" and not "ValueTask")
      return false;

    if (!IsSystemThreadingTasksNamespace(type.ContainingNamespace))
      return false;

    return true;
  }

  public static bool IsCancellationTokenType(this ITypeSymbol type)
  {
    if (!type.IsValueType)
      return false;

    if (type.Name != "CancellationToken")
      return false;

    if (!IsSystemThreadingNamespace(type.ContainingNamespace))
      return false;

    return true;
  }

  public static bool IsCollectionLikeType(this INamedTypeSymbol type)
  {
    if (!type.IsGenericType)
      return false;

    if (!IsSystemCollectionsGenericNamespace(type.ContainingNamespace))
      return false;

    return type.Name is
      "IEnumerable"         or
      "IReadOnlyCollection" or
      "ICollection"         or
      "IReadOnlyList"       or
      "IList"               or
      "List"                or
      "LinkedList"          or
      "ISet"                or
      "HashSet"             or
      "SortedSet"           or
      "Queue";
  }

  public static bool IsDictionaryLikeType(this INamedTypeSymbol type)
  {
    if (!type.IsGenericType)
      return false;

    if (!IsSystemCollectionsGenericNamespace(type.ContainingNamespace))
      return false;

    return type.Name is
      "IReadOnlyDictionary" or
      "IDictionary"         or
      "Dictionary"          or
      "SortedDictionary"    or
      "KeyValuePair";
  }

  public static bool IsSupportedKeyType(this ITypeSymbol type)
  {
    if (!IsSystemNamespace(type.ContainingNamespace))
      return false;

    return type.Name is
      "Boolean"        or
      "Byte"           or
      "DateTime"       or
      "DateTimeOffset" or
      "Decimal"        or
      "Double"         or
      "Guid"           or
      "Int16"          or
      "Int32"          or
      "Int64"          or
      "Object"         or
      "SByte"          or
      "Single"         or
      "String"         or
      "UInt16"         or
      "UInt32"         or
      "UInt64"         ||
      type.BaseType is { } baseType && IsSystemNamespace(baseType.ContainingNamespace) && baseType.Name == "Enum";
  }

  public static Location GetLocation(this AttributeData attribute)
  {
    return attribute.ApplicationSyntaxReference is { } syntaxReference
      ? Location.Create(syntaxReference.SyntaxTree, syntaxReference.Span)
      : Location.None;
  }

  public static bool IsCodeArchitectsPlatformActorsCodeAnalysisNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "CodeAnalysis" && IsCodeArchitectsPlatformActorsNamespace(@namespace.ContainingNamespace);
  }

  public static bool IsCodeArchitectsPlatformActorsMessagingNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "Messaging" && IsCodeArchitectsPlatformActorsNamespace(@namespace.ContainingNamespace);
  }

  public static bool IsCodeArchitectsPlatformActorsNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "Actors" && IsCodeArchitectsPlatformNamespace(@namespace.ContainingNamespace);
  }

  public static bool IsCodeArchitectsPlatformMessagingNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "Messaging" && IsCodeArchitectsPlatformNamespace(@namespace.ContainingNamespace);
  }

  public static bool IsCodeArchitectsPlatformNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "Platform" && IsCodeArchitectsNamespace(@namespace.ContainingNamespace);
  }

  public static bool IsCodeArchitectsNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "CodeArchitects" && @namespace.ContainingNamespace.IsGlobalNamespace;
  }

  public static bool IsSystemThreadingTasksNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "Tasks" && IsSystemThreadingNamespace(@namespace.ContainingNamespace);
  }

  public static bool IsSystemThreadingNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "Threading" && IsSystemNamespace(@namespace.ContainingNamespace);
  }

  public static bool IsSystemCollectionsGenericNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "Generic" && IsSystemCollectionsNamespace(@namespace.ContainingNamespace);
  }

  public static bool IsSystemCollectionsNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "Collections" && IsSystemNamespace(@namespace.ContainingNamespace);
  }

  public static bool IsSystemNamespace(this INamespaceSymbol? @namespace)
  {
    return @namespace is not null && @namespace.Name == "System" && @namespace.ContainingNamespace.IsGlobalNamespace;
  }
}
