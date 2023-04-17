using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Generator;

internal readonly ref partial struct ActorDescriptorFactory
{
  private void Analyze(ITypeSymbol targetType, ActorData? actor, List<FactoryData>? factories, List<ImplementationData>? implementations)
  {
    if (!TargetIsActor(targetType, actor, factories, implementations, out INamedTypeSymbol? actorType))
      return;

    AnalyzeAttributes(actor.ActorAttribute, actor.GenericActorAttribute);
    AnalyzeAttributes(actor.IdTypeAttribute, actor.GenericIdTypeAttribute);
    bool isGeneric = AnalyzeArity(actorType);

    INamedTypeSymbol? interfaceType = FindInterfaceType(actor);
    bool hasInterface = interfaceType is not null;
    if (hasInterface)
    {
      AnalyzeInterface(interfaceType!, actorType);
    }

    _ = SymbolExtensions.TryGetAttributeTargetType(actor.IdTypeAttribute, actor.GenericIdTypeAttribute, out ITypeSymbol? actorIdType);
    if (actorIdType is not null && !IsValidIdType(actorIdType))
    {
      InvalidIdType(actor.GenericIdTypeAttribute ?? actor.IdTypeAttribute!, actorIdType);
      actorIdType = null;
    }

    MemberAnalysisResult memberAnalysisResult = AnalyzeMembers(actor, actorIdType);
    List<IFieldSymbol> stateFields = memberAnalysisResult.StateFields;
    ITypeSymbol? idType = actorIdType ?? memberAnalysisResult.MemberIdType;

    bool isVirtual = AnalyzeVirtualActor(actor, stateFields, memberAnalysisResult.CanBeVirtual);
    List<StateDependencyDescriptor>? stateDependencies = AnalyzeConstructor(actorType, stateFields);

    AnalyzeMethods(actorType);
    AnalyzeActorMessages(actor, idType);

    bool hasFactory = AnalyzeFactories(actorType, factories, interfaceType, idType, stateFields, memberAnalysisResult.AmbiguousIdType, isVirtual, memberAnalysisResult.GetsIdFromState);
    AnalyzeImplementations(actor, memberAnalysisResult.IdMember, idType, implementations);

    if (isGeneric || hasFactory || !hasInterface || stateDependencies is null)
      return;

    AddDescriptor(actorType, interfaceType!, idType, isVirtual, memberAnalysisResult.GetsIdFromState, stateDependencies);
  }

  private bool TargetIsActor(ITypeSymbol targetType, [NotNullWhen(true)] ActorData? actor, List<FactoryData>? factories, List<ImplementationData>? implementations, [NotNullWhen(true)] out INamedTypeSymbol? actorType)
  {
    if (targetType is not INamedTypeSymbol namedType || actor is null)
    {
      if (factories is not null)
      {
        foreach (FactoryData factory in factories)
        {
          TargetIsNotAnActor(factory.FactoryType, targetType);
        }
      }

      if (implementations is not null)
      {
        foreach (ImplementationData implementation in implementations)
        {
          TargetIsNotAnActor(implementation.ImplementationType, targetType);
        }
      }

      actorType = null;
      return false;
    }

    actorType = namedType;
    return true;
  }

  private INamedTypeSymbol? FindInterfaceType(ActorData data)
  {
    _ = SymbolExtensions.TryGetInterfaceType(data.ActorAttribute, data.GenericActorAttribute, out ITypeSymbol? specifiedInterfaceType);

    return specifiedInterfaceType is not null
      ? GetSpecifiedInterfaceType(data, specifiedInterfaceType)
      : InferInterfaceType(data);
  }

  private INamedTypeSymbol? GetSpecifiedInterfaceType(ActorData data, ITypeSymbol specifiedInterfaceType)
  {
    if (specifiedInterfaceType.TypeKind is TypeKind.Error)
      return null;

    INamedTypeSymbol actorType = data.ActorType;
    ImmutableArray<INamedTypeSymbol> interfaceTypes = actorType.AllInterfaces;

    if (specifiedInterfaceType.TypeKind is not TypeKind.Interface)
    {
      InvalidInterfaceType(data.GenericActorAttribute ?? data.ActorAttribute!, specifiedInterfaceType);
      return null;
    }

    INamespaceSymbol baseNamespace = specifiedInterfaceType.GetBaseNamespace();

    if (!baseNamespace.IsGlobalNamespace && baseNamespace.IsCodeArchitectsNamespace())
    {
      InvalidInterfaceType(data.GenericActorAttribute ?? data.ActorAttribute!, specifiedInterfaceType);
      return null;
    }

    foreach (INamedTypeSymbol candidateInterfaceType in interfaceTypes)
    {
      if (SymbolEqualityComparer.Default.Equals(candidateInterfaceType, specifiedInterfaceType))
        return candidateInterfaceType;
    }

    InterfaceNotImplemented(actorType, data.ActorAttribute ?? data.GenericActorAttribute!);
    return (INamedTypeSymbol)specifiedInterfaceType;
  }

  private INamedTypeSymbol? InferInterfaceType(ActorData data)
  {
    INamedTypeSymbol actorType = data.ActorType;
    INamespaceSymbol actorBaseNamespace = actorType.GetBaseNamespace();
    ImmutableArray<INamedTypeSymbol> interfaceTypes = actorType.AllInterfaces;

    INamedTypeSymbol? interfaceType = null;
    foreach (INamedTypeSymbol candidateInterfaceType in interfaceTypes)
    {
      INamespaceSymbol baseNamespace = candidateInterfaceType.GetBaseNamespace();

      if (!SymbolEqualityComparer.Default.Equals(baseNamespace, actorBaseNamespace))
        continue;

      if (interfaceType is not null)
      {
        AmbiguousActorInterface(actorType);
        return null;
      }

      interfaceType = candidateInterfaceType;
    }

    if (interfaceType is null)
    {
      MissingActorInterface(actorType);
    }

    return interfaceType;
  }

  private bool AnalyzeArity(INamedTypeSymbol implementationType)
  {
    if (implementationType.IsGenericType)
    {
      GenericActorsAreNotSupported(implementationType);
      return true;
    }

    return false;
  }

  private void AnalyzeInterface(INamedTypeSymbol interfaceType, INamedTypeSymbol actorType)
  {
    if (_disableActorDiagnostics)
      return;

    bool definesProperties = false;
    bool definesEvents = false;

    foreach (ISymbol interfaceMember in interfaceType.GetMembers())
    {
      if (interfaceMember.Kind is not SymbolKind.Method)
      {
        definesProperties |= interfaceMember.Kind is SymbolKind.Property;
        definesEvents |= interfaceMember.Kind is SymbolKind.Event;
        continue;
      }

      IMethodSymbol interfaceMethod = (IMethodSymbol)interfaceMember;

      if (interfaceMethod.MethodKind is not MethodKind.Ordinary)
        continue;

      if (actorType.FindImplementationForInterfaceMember(interfaceMethod) is not IMethodSymbol implementationMethod)
        continue;

      if (implementationMethod.IsGenericMethod)
      {
        GenericMethodsAreNotSupported(implementationMethod);
        continue;
      }

      if (!implementationMethod.ReturnType.IsTaskLikeType())
      {
        InvalidMethodReturnType(implementationMethod);
      }
    }

    if (definesProperties)
    {
      PropertiesAreNotSupported(actorType, interfaceType);
    }

    if (definesEvents)
    {
      EventsAreNotSupported(actorType, interfaceType);
    }
  }

  private void AnalyzeAttributes(AttributeData? nonGenericAttribute, AttributeData? genericAttribute)
  {
    if (nonGenericAttribute is not null && genericAttribute is not null)
    {
      DuplicateAttribute(nonGenericAttribute);
    }
  }

  private void AnalyzeMethods(INamedTypeSymbol implementationType)
  {
    if (_disableActorDiagnostics)
      return;

    foreach (ISymbol member in implementationType.GetMembers())
    {
      if (member is not IMethodSymbol method || method.MethodKind is not MethodKind.Ordinary || method.IsOverride)
        continue;

      ITypeSymbol returnType = method.ReturnType;
      if (!returnType.IsTaskLikeType() && returnType.SpecialType is not SpecialType.System_Void)
        continue;

      AnalyzeCancellationTokenParameter(method);
    }
  }

  private void AnalyzeCancellationTokenParameter(IMethodSymbol implementationMethod)
  {
    ImmutableArray<IParameterSymbol> parameters = implementationMethod.Parameters;

    for (int i = 0; i < parameters.Length - 1; i++)
    {
      IParameterSymbol parameter = parameters[i];
      if (parameter.Type.IsCancellationTokenType())
      {
        CancellationTokenMustBeLastParameter(implementationMethod, parameter);
      }
    }
  }

  private MemberAnalysisResult AnalyzeMembers(ActorData actor, ITypeSymbol? actorIdType)
  {
    INamedTypeSymbol actorType = actor.ActorType;
    List<IFieldSymbol> stateFields = new();
    ISymbol? idMember = null;
    ITypeSymbol? memberIdType = null;
    bool ambiguousIdType = false;
    bool multipleIdMembers = false;
    bool canBeVirtual = true;
    bool getsIdFromState = false;

    foreach (ISymbol member in actorType.GetMembers())
    {
      if (member is not IFieldSymbol field)
        continue;

      if (field.GetActorMember() is not ISymbol candidateMember)
        continue;

      ITypeSymbol memberType = field.Type;
      (AttributeData? stateAttribute, AttributeData? actorIdAttribute) = candidateMember.GetMemberAttributes();
      bool isState = stateAttribute is not null;
      bool isActorId = actorIdAttribute is not null;

      if (isState)
      {
        AnalyzeStateType(field, stateAttribute!);
        AnalyzeDefaultValue(field, stateAttribute!, ref canBeVirtual);

        stateFields.Add(field);
      }

      if (isActorId)
      {
        AnalyzeActorIdMember(actor, actorIdType, candidateMember, memberType);

        getsIdFromState |= isState;
        multipleIdMembers |= idMember is not null;
        ambiguousIdType |= memberIdType is not null && !SymbolEqualityComparer.Default.Equals(memberIdType, memberType);

        idMember = candidateMember;
        memberIdType = memberType;
      }
    }

    if (multipleIdMembers)
    {
      MultipleIdMembers(actorType);
    }

    if (ambiguousIdType)
    {
      idMember = null;
      memberIdType = null;
    }

    return new(stateFields, ambiguousIdType, idMember, memberIdType, getsIdFromState, canBeVirtual);
  }

  private void AnalyzeStateType(IFieldSymbol stateField, AttributeData stateAttribute)
  {
    if (!IsValidStateType(stateField.Type))
    {
      InvalidStateType(stateField, stateAttribute);
    }

    static bool IsValidStateType(ITypeSymbol type)
    {
      if (type is not INamedTypeSymbol namedType)
      {
        return type is IArrayTypeSymbol arrayType && IsValidStateType(arrayType.ElementType);
      }

      if (namedType.IsGenericType)
      {
        INamedTypeSymbol typeDefinition = namedType.OriginalDefinition;
        ITypeSymbol firstTypeArgument = namedType.TypeArguments[0];

        if (typeDefinition.IsCollectionLikeType())
          return IsValidStateType(firstTypeArgument);

        if (typeDefinition.IsDictionaryLikeType())
          return firstTypeArgument.IsSupportedKeyType() && IsValidStateType(namedType.TypeArguments[0]);
      }

      return type.TypeKind is not TypeKind.Interface && !type.IsAbstract;
    }
  }

  private void AnalyzeDefaultValue(IFieldSymbol stateField, AttributeData stateAttribute, ref bool canBeVirtual)
  {
    if (!stateAttribute.HasDefaultValue(out TypedConstant defaultValue))
    {
      ITypeSymbol memberType = stateField.Type;
      canBeVirtual &= memberType.IsValueType || memberType.HasParameterlessConstructor();
      return;
    }

    if (defaultValue.Value is null)
    {
      if (stateField.Type.IsValueType)
      {
        InvalidDefaultValue(stateField, stateAttribute);
      }
    }
    else if (defaultValue.Type is { } type && !SymbolEqualityComparer.Default.Equals(type, stateField.Type))
    {
      InvalidDefaultValue(stateField, stateAttribute);
    }
  }

  private void AnalyzeActorIdMember(ActorData actor, ITypeSymbol? actorIdType, ISymbol candidateMember, ITypeSymbol memberType)
  {
    if (!IsValidIdType(memberType))
    {
      InvalidIdType(candidateMember, memberType);
    }

    if (actorIdType is not null && !SymbolEqualityComparer.Default.Equals(memberType, actorIdType))
    {
      InvalidIdMember(candidateMember, memberType, actorIdType, actor.GenericIdTypeAttribute ?? actor.IdTypeAttribute, null);
    }
  }

  private List<StateDependencyDescriptor>? AnalyzeConstructor(INamedTypeSymbol actorType, List<IFieldSymbol> stateFields)
  {
    if (!TryGetConstructor(actorType, out IMethodSymbol? constructor))
      return null;

    int stateArity = stateFields.Count;
    int index = 0;
    List<StateDependencyDescriptor> stateDependencies = new(stateArity);
    bool hasStateComponentMismatch = false;

    foreach (IParameterSymbol parameter in constructor.Parameters)
    {
      if (IsStateParameter(parameter, stateFields, ref hasStateComponentMismatch))
      {
        stateDependencies.Add(new StateDependencyDescriptor(
          parameter.Type.ToDisplayString(Format.GlobalFullName),
          parameter.Name));

        index++;
        continue;
      }

      AnalyzeActorContext(actorType, parameter);
    }

    if (hasStateComponentMismatch || index != stateFields.Count)
    {
      StateComponentNameMismatch(constructor);
    }

    return stateDependencies;
  }

  private void AnalyzeActorContext(INamedTypeSymbol actorType, IParameterSymbol parameter)
  {
    ITypeSymbol parameterType = parameter.Type;

    if (!parameterType.ContainingNamespace.IsCodeArchitectsPlatformActorsNamespace())
      return;

    if (parameterType.Name == "IActorContext")
    {
      INamedTypeSymbol namedParameterType = (INamedTypeSymbol)parameterType;
      if (namedParameterType.IsGenericType && !SymbolEqualityComparer.Default.Equals(((INamedTypeSymbol)parameter.Type).TypeArguments[0], actorType))
      {
        WrongGenericActorContext(parameter, actorType);
      }
    }
  }

  private bool TryGetConstructor(INamedTypeSymbol actorType, [NotNullWhen(true)] out IMethodSymbol? constructor)
  {
    ImmutableArray<IMethodSymbol> constructors = actorType.InstanceConstructors;
    if (constructors.Length == 1)
    {
      constructor = constructors[0];
      return true;
    }

    constructor = null;
    foreach (IMethodSymbol candidateConstructor in constructors)
    {
      if (candidateConstructor.HasActorConstructorAttribute())
      {
        if (constructor is not null)
        {
          AmbiguousActorConstructor(actorType);

          constructor = null;
          return false;
        }

        constructor = candidateConstructor;
      }
    }

    if (constructor is null)
    {
      AmbiguousActorConstructor(actorType);
      return false;
    }

    return true;
  }

  private bool AnalyzeVirtualActor(ActorData actor, List<IFieldSymbol> stateFields, bool canBeVirtual)
  {
    bool isExplicitVirtual = actor.VirtualAttribute is not null;

    if (!canBeVirtual && isExplicitVirtual)
    {
      ActorCannotBeVirtual(actor.ActorType, actor.VirtualAttribute!);
    }

    return isExplicitVirtual || stateFields.Count == 0;
  }

  private void AnalyzeActorMessages(ActorData actor, ITypeSymbol? idType)
  {
    if (_disableActorDiagnostics)
      return;

    foreach (INamedTypeSymbol interfaceType in actor.ActorType.AllInterfaces)
    {
      if (!interfaceType.ContainingNamespace.IsCodeArchitectsPlatformMessagingNamespace() || interfaceType.Name != "IMessageHandler")
        continue;

      ITypeSymbol messageType = interfaceType.TypeArguments[0];

      INamedTypeSymbol? actorMessageInterfaceType = null;
      foreach (INamedTypeSymbol messageInterfaceType in messageType.AllInterfaces)
      {
        if (!messageInterfaceType.ContainingNamespace.IsCodeArchitectsPlatformActorsMessagingNamespace() || messageInterfaceType.Name != "IActorMessage")
          continue;

        actorMessageInterfaceType = messageInterfaceType;
        break;
      }

      if (actorMessageInterfaceType is null)
      {
        InvalidActorMessage(actor, interfaceType, messageType, idType);
        continue;
      }

      ITypeSymbol messageIdType = actorMessageInterfaceType.TypeArguments[0];

      if (idType is null)
      {
        if (messageIdType.SpecialType is not SpecialType.System_String)
        {
          InvalidActorMessage(actor, interfaceType, messageType, idType);
          continue;
        }
      }
      else
      {
        if (!SymbolEqualityComparer.Default.Equals(messageIdType, idType))
        {
          InvalidActorMessage(actor, interfaceType, messageType, idType);
          continue;
        }
      }
    }
  }

  private void AnalyzeImplementations(ActorData actor, ISymbol? idMember, ITypeSymbol? idType, List<ImplementationData>? implementations)
  {
    if (_disableActorDiagnostics)
      return;

    INamedTypeSymbol actorType = actor.ActorType;
    if (implementations is null || implementations.Count == 0)
    {
      if (actorType.IsAbstract)
      {
        AbstractImplementation(actorType);
      }
      return;
    }

    bool hasDefaultImplementation = false;
    bool hasMultipleDefaultImplementations = false;

    foreach (ImplementationData implementation in implementations)
    {
      AnalyzeImplementation(actor, idMember, idType, implementation, ref hasDefaultImplementation, ref hasMultipleDefaultImplementations);
    }

    if (hasMultipleDefaultImplementations)
    {
      MultipleDefaultImplementations(actorType);
    }
  }

  private void AnalyzeImplementation(ActorData actor, ISymbol? idMember, ITypeSymbol? idType, ImplementationData implementation, ref bool hasDefaultImplementation, ref bool hasMultipleDefaultImplementations)
  {
    INamedTypeSymbol actorType = actor.ActorType;
    INamedTypeSymbol implementationType = implementation.ImplementationType;

    if (implementationType.IsAbstract)
    {
      AbstractImplementation(implementationType);
    }

    AnalyzeAttributes(implementation.ImplementationAttribute, implementation.GenericImplementationAttribute);
    AnalyzeArity(implementationType);

    if (!InheritsActor(implementationType, actorType))
    {
      ActorNotInherited(implementationType, implementation.GenericImplementationAttribute ?? implementation.ImplementationAttribute!);
    }

    foreach (ISymbol member in implementationType.GetMembers())
    {
      if (member is not IFieldSymbol field)
        continue;

      if (field.GetActorMember() is not ISymbol candidateMember)
        continue;

      ITypeSymbol memberType = field.Type;

      foreach (AttributeData attribute in member.GetAttributes())
      {
        if (attribute.AttributeClass is not INamedTypeSymbol attributeType)
          continue;

        if (!attributeType.ContainingNamespace.IsCodeArchitectsPlatformActorsNamespace())
          continue;

        if (attributeType.Name is "StateAttribute")
        {
          StateMustBeDefinedInBaseActor(implementationType, attribute);
          continue;
        }

        if (attributeType.Name is "ActorIdAttribute" && idType is not null && !SymbolEqualityComparer.Default.Equals(memberType, idType))
        {
          InvalidIdMember(candidateMember, memberType, idType, actor.GenericIdTypeAttribute ?? actor.IdTypeAttribute, idMember);
        }
      }
    }

    AnalyzeMethods(implementationType);

    AttributeData implementationAttribute = implementation.ImplementationAttribute ?? implementation.GenericImplementationAttribute!;

    foreach (KeyValuePair<string, TypedConstant> arg in implementationAttribute.NamedArguments)
    {
      if (arg.Key != "IsDefault" || arg.Value.Value is not bool argumentValue || !argumentValue)
        continue;

      hasMultipleDefaultImplementations = hasDefaultImplementation;
      hasDefaultImplementation = true;
    }
  }

  private bool InheritsActor(INamedTypeSymbol implementationType, INamedTypeSymbol actorType)
  {
    ITypeSymbol? baseType = implementationType.BaseType;

    while (baseType is not null)
    {
      if (!baseType.IsNonErrorOrWrongArityErrorType(out baseType))
        return false;

      if (SymbolEqualityComparer.Default.Equals(baseType, actorType))
        return true;

      baseType = baseType.BaseType;
    }

    return false;
  }

  private bool AnalyzeFactories(INamedTypeSymbol actorType, List<FactoryData>? factories, INamedTypeSymbol? interfaceType, ITypeSymbol? idType, List<IFieldSymbol> stateFields, bool ambiguousIdType, bool isVirtual, bool getsIdFromState)
  {
    if (_disableActorDiagnostics)
      return factories is not null && factories.Count != 0;

    if (factories is null || factories.Count == 0)
    {
      if (_disableActorFactoryGeneration)
      {
        MissingActorFactoryType(actorType);
      }

      return false;
    }

    if (factories.Count > 1)
    {
      AmbiguousActorFactoryType(actorType);
    }

    foreach (FactoryData factory in factories)
    {
      AnalyzeFactory(factory, interfaceType, idType, stateFields, ambiguousIdType, isVirtual, getsIdFromState);
    }

    return true;
  }

  private void AnalyzeFactory(FactoryData factory, INamedTypeSymbol? interfaceType, ITypeSymbol? idType, List<IFieldSymbol> stateFields, bool ambiguousIdType, bool isVirtual, bool getsIdFromState)
  {
    INamedTypeSymbol factoryType = factory.FactoryType;
    int stateArity = stateFields.Count;

    AnalyzeAttributes(factory.FactoryAttribute, factory.GenericFactoryAttribute);

    if (factoryType.IsGenericType)
    {
      GenericActorFactoriesAreNotSupported(factoryType);
      return;
    }

    IMethodSymbol? getMethod = null;
    IMethodSymbol? createAsyncMethod = null;

    foreach (ISymbol member in factoryType.GetMembers())
    {
      if (member.Kind is not SymbolKind.Method)
      {
        InvalidActorFactoryType(factoryType);
        return;
      }

      if (member.Name is "Get")
      {
        getMethod = (IMethodSymbol)member;
        continue;
      }

      if (member.Name is "CreateAsync")
      {
        createAsyncMethod = (IMethodSymbol)member;
        continue;
      }

      InvalidActorFactoryType(factoryType);
      return;
    }

    if (getMethod is null || !isVirtual && createAsyncMethod is null)
    {
      InvalidActorFactoryType(factoryType);
      return;
    }

    if (!SymbolEqualityComparer.Default.Equals(getMethod.ReturnType, interfaceType))
    {
      InvalidActorFactoryType(factoryType);
      return;
    }

    if (getMethod.Parameters.Length != 1 || !MatchesIdType(getMethod.Parameters[0].Type, idType))
    {
      InvalidActorFactoryType(factoryType);
      return;
    }

    if (stateArity == 0)
      return;

    if (!createAsyncMethod!.ReturnType.IsGenericTaskType(out ITypeSymbol? resultType) || !SymbolEqualityComparer.Default.Equals(resultType, interfaceType))
    {
      InvalidActorFactoryType(factoryType);
      return;
    }

    ImmutableArray<IParameterSymbol> createAsyncParameters = createAsyncMethod.Parameters;

    bool createAsyncSignatureMatches =
      createAsyncParameters.Length == 2 + stateArity &&
      MatchesIdType(createAsyncParameters[0].Type, idType) &&
      createAsyncParameters[createAsyncParameters.Length - 1].Type.IsCancellationTokenType();

    if (!createAsyncSignatureMatches)
    {
      InvalidActorFactoryType(factoryType);
      return;
    }

    for (int i = 0; i < stateArity; i++)
    {
      if (!SymbolEqualityComparer.Default.Equals(createAsyncParameters[i + 1].Type, stateFields[i].Type))
      {
        InvalidActorFactoryType(factoryType);
        return;
      }
    }

    static bool MatchesIdType(ITypeSymbol type, ITypeSymbol? idType)
    {
      return idType is null
        ? type.SpecialType is SpecialType.System_String
        : SymbolEqualityComparer.Default.Equals(type, idType);
    }
  }

  private static bool IsValidIdType(ITypeSymbol idType)
  {
    if (idType.SpecialType is SpecialType.System_String)
      return true;

    foreach (ISymbol member in idType.GetMembers())
    {
      if (!member.IsStatic || member.Name != "Parse" || member is not IMethodSymbol method)
        continue;

      if (!SymbolEqualityComparer.Default.Equals(method.ReturnType, idType))
        continue;

      ImmutableArray<IParameterSymbol> parameters = method.Parameters;

      if (parameters.Length is not 1 and not 2)
        continue;

      if (parameters[0].Type.SpecialType is not SpecialType.System_String)
        continue;

      if (parameters.Length == 1)
        return true;

      if (!parameters[1].Type.ContainingNamespace.IsSystemNamespace() || parameters[1].Type.Name != "IFormatProvider")
        continue;
    }

    return false;
  }

  private static bool IsStateParameter(IParameterSymbol parameter, List<IFieldSymbol> stateFields, ref bool hasStateComponentMismatch)
  {
    string parameterName = parameter.Name;
    bool found = false;

    foreach (IFieldSymbol stateField in stateFields)
    {
      string fieldName = stateField.Name;

      bool namesMatch =
        parameterName.MatchesUnderscorePrefixConvention(fieldName) ||
        parameterName.Equals(fieldName) ||
        (stateField.AssociatedSymbol is ISymbol associatedSymbol && associatedSymbol.Name.MatchesCamelCaseConvention(parameterName)) ||
        parameterName.MatchesMemberPrefixConvention(fieldName);

      if (namesMatch && SymbolEqualityComparer.Default.Equals(parameter.Type, stateField.Type))
      {
        if (found)
        {
          hasStateComponentMismatch = true;
          break;
        }

        found = true;
      }
    }

    return found;
  }

  private record MemberAnalysisResult(
    List<IFieldSymbol> StateFields,
    bool AmbiguousIdType,
    ISymbol? IdMember,
    ITypeSymbol? MemberIdType,
    bool GetsIdFromState,
    bool CanBeVirtual);
}
