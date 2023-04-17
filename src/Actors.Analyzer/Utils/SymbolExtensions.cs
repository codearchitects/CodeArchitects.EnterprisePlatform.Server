using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Utils;

internal static class SymbolExtensions
{
  public static bool IsAccessible(this ITypeSymbol type, SemanticModel semanticModel, int position)
  {
    return
      (type.SpecialType is >= SpecialType.System_Boolean and <= SpecialType.System_String) ||
      semanticModel.LookupNamespacesAndTypes(position).Contains(type);
  }

  public static INamespaceSymbol GetBaseNamespace(this ITypeSymbol type)
  {
    INamespaceSymbol @namespace = type.ContainingNamespace;

    if (@namespace.IsGlobalNamespace)
      return @namespace;

    while (!@namespace.ContainingNamespace.IsGlobalNamespace)
    {
      @namespace = @namespace.ContainingNamespace;
    }

    return @namespace;
  }

  public static bool TryGetAttributeTargetType(AttributeData? nonGenericAttribute, AttributeData? genericAttribute, [NotNullWhen(true)] out ITypeSymbol? type)
  {
    if (!TryGetAttributeTargetTypeCore(nonGenericAttribute, genericAttribute, out type))
      return false;

    return type.IsNonErrorOrWrongArityErrorType(out type);
  }

  public static bool TryGetAttributeTargetType(this AttributeData attribute, [NotNullWhen(true)] out ITypeSymbol? type)
  {
    if (!TryGetAttributeTargetTypeCore(attribute, out type))
      return false;

    return type.IsNonErrorOrWrongArityErrorType(out type);
  }

  private static bool TryGetAttributeTargetTypeCore(AttributeData? nonGenericAttribute, AttributeData? genericAttribute, [NotNullWhen(true)] out ITypeSymbol? type)
  {
    type = null;

    if (genericAttribute is null)
    {
      if (nonGenericAttribute is null)
        return false;

      return TryGetNonGenericAttributeTargetType(nonGenericAttribute, out type);
    }

    if (genericAttribute.AttributeClass is not INamedTypeSymbol attributeClass)
      return false;

    return TryGetGenericAttributeTargetType(attributeClass, out type);
  }

  private static bool TryGetAttributeTargetTypeCore(this AttributeData attribute, [NotNullWhen(true)] out ITypeSymbol? type)
  {
    type = null;

    if (attribute.AttributeClass is not INamedTypeSymbol attributeClass)
      return false;

    if (attributeClass.IsGenericType)
      return TryGetGenericAttributeTargetType(attributeClass, out type);

    return TryGetNonGenericAttributeTargetType(attribute, out type);
  }

  public static bool IsNonErrorOrWrongArityErrorType(this ITypeSymbol type, [NotNullWhen(true)] out ITypeSymbol? nonErrorType)
  {
    if (type is not IErrorTypeSymbol errorType)
    {
      nonErrorType = type;
      return true;
    }

    if (errorType.CandidateReason is CandidateReason.WrongArity && errorType.CandidateSymbols is [ITypeSymbol candidateType])
    {
      nonErrorType = candidateType;
      return true;
    }

    nonErrorType = null;
    return false;
  }

  public static bool TryGetNonGenericAttributeTargetType(AttributeData attribute, [NotNullWhen(true)] out ITypeSymbol? type)
  {
    type = null;

    if (attribute.ConstructorArguments.Length == 0)
      return false;

    if (attribute.ConstructorArguments[0].Value is not ITypeSymbol argumentValue)
      return false;

    type = argumentValue;
    return true;
  }

  public static bool TryGetGenericAttributeTargetType(INamedTypeSymbol attributeClass, [NotNullWhen(true)] out ITypeSymbol? type)
  {
    type = null;

    if (attributeClass.TypeArguments.Length == 0)
      return false;

    if (attributeClass.TypeArguments[0] is not ITypeSymbol typeArgument)
      return false;

    type = typeArgument;
    return true;
  }

  public static bool TryGetInterfaceType(AttributeData? actorAttribute, AttributeData? genericActorAttribute, out ITypeSymbol? interfaceType)
  {
    interfaceType = null;

    if (genericActorAttribute is null || genericActorAttribute.AttributeClass is not INamedTypeSymbol attributeClass)
    {
      if (actorAttribute is null)
        return false;

      foreach (KeyValuePair<string, TypedConstant> namedArgument in actorAttribute.NamedArguments)
      {
        if (namedArgument.Key is "InterfaceType")
        {
          if (namedArgument.Value.Value is not ITypeSymbol argumentValue)
            return false;

          interfaceType = argumentValue;
          return true;
        }
      }

      return true;
    }

    if (attributeClass.TypeArguments.Length == 0)
      return false;

    interfaceType = attributeClass.TypeArguments[0];
    return true;
  }

  public static ISymbol? GetActorMember(this IFieldSymbol field)
  {
    if (field.AssociatedSymbol is not ISymbol associatedSymbol)
      return field;

    return associatedSymbol.Kind is not SymbolKind.Property
      ? null
      : associatedSymbol;
  }

  public static (AttributeData? StateAttribute, AttributeData? ActorIdAttribute) GetMemberAttributes(this ISymbol member)
  {
    AttributeData? stateAttribute = null;
    AttributeData? actorIdAttribute = null;
    foreach (AttributeData attribute in member.GetAttributes())
    {
      if (attribute.AttributeClass is not INamedTypeSymbol attributeType)
        continue;

      if (!attributeType.ContainingNamespace.IsCodeArchitectsPlatformActorsNamespace())
        continue;

      switch (attributeType.Name)
      {
        case "StateAttribute":
          stateAttribute = attribute;
          break;
        case "ActorIdAttribute":
          actorIdAttribute = attribute;
          break;
      }
    }

    return (stateAttribute, actorIdAttribute);
  }

  public static bool HasParameterlessConstructor(this ITypeSymbol type)
  {
    if (type is not INamedTypeSymbol namedType)
      return false;

    foreach (IMethodSymbol constructor in namedType.InstanceConstructors)
    {
      if (constructor.Parameters.Length == 0)
        return true;
    }

    return false;
  }

  public static bool HasDefaultValue(this AttributeData stateAttribute, [NotNullWhen(true)] out TypedConstant defaultValue)
  {
    foreach (KeyValuePair<string, TypedConstant> namedArgument in stateAttribute.NamedArguments)
    {
      if (namedArgument.Key is "Default")
      {
        defaultValue = namedArgument.Value;
        return true;
      }
    }

    defaultValue = default;
    return false;
  }

  public static bool HasActorConstructorAttribute(this IMethodSymbol constructor)
  {
    foreach (AttributeData attribute in constructor.GetAttributes())
    {
      if (attribute.IsActorConstructorAttribute())
        return true;
    }

    return false;
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
