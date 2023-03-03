using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Descriptors;

internal readonly ref partial struct ActorDescriptorFactory
{
  private readonly bool _disableDiagnostics;
  private readonly bool _disableFactoryGeneration;
  private readonly ICollection<ActorDescriptor> _descriptors;
  private readonly Dictionary<INamedTypeSymbol, ActorDataEntry> _actorDataDictionary;
  private readonly ICollection<Diagnostic> _diagnostics;

  public ActorDescriptorFactory(ActorAnalyzerOptions options)
  {
    _disableFactoryGeneration = (options & ActorAnalyzerOptions.DisableFactoryGeneration) != 0;
    _disableDiagnostics = (options & ActorAnalyzerOptions.DisableDiagnostics) != 0;

    _descriptors = _disableFactoryGeneration ? Array.Empty<ActorDescriptor>() : new List<ActorDescriptor>();
    _actorDataDictionary = new(SymbolEqualityComparer.Default);
    _diagnostics = _disableDiagnostics ? Array.Empty<Diagnostic>() : new List<Diagnostic>();
  }

  public DiagnosticResult<RecordList<ActorDescriptor>> GetResult()
  {
    return new(
      new RecordList<ActorDescriptor>((IReadOnlyList<ActorDescriptor>)_descriptors),
      new RecordList<Diagnostic>((IReadOnlyList<Diagnostic>)_diagnostics));
  }

  public void AddFactoryData(FactoryData factoryData)
  {
    ITypeSymbol actorType = factoryData.GetSpecifiedActorType(in this);
    if (actorType is not INamedTypeSymbol namedActorType)
      return;

    if (_actorDataDictionary.TryGetValue(namedActorType, out ActorDataEntry entry))
    {
      if (entry.Factory is not null)
      {
        AmbiguousActorFactoryType(namedActorType, factoryData.Type);
        return;
      }

      _actorDataDictionary[namedActorType] = entry with { Factory = factoryData };
      return;
    }

    _actorDataDictionary.Add(namedActorType, new ActorDataEntry(factoryData, null));
  }

  public void AddImplementationData(ImplementationData implementationData)
  {
    ITypeSymbol actorType = implementationData.GetSpecifiedActorType(in this);
    if (actorType is not INamedTypeSymbol namedActorType)
    {
      InvalidImplementation(implementationData.Type);
      return;
    }

    INamedTypeSymbol implementationType = implementationData.Type;

    if (implementationType.IsAbstract)
    {
      AbstractImplementation(implementationType);
    }

    INamedTypeSymbol baseType = implementationType;
    do
    {
      baseType = baseType.BaseType!;
    }
    while (baseType.SpecialType is not SpecialType.System_Object && !SymbolEqualityComparer.Default.Equals(baseType, namedActorType));

    if (baseType.SpecialType is SpecialType.System_Object)
    {
      InvalidImplementation(implementationType);
      return;
    }

    if (_actorDataDictionary.TryGetValue(namedActorType, out ActorDataEntry entry))
    {
      if (entry.Implementations is not null)
      {
        entry.Implementations.Add(implementationData);
        return;
      }

      _actorDataDictionary[namedActorType] = entry with { Implementations = new List<ImplementationData>() { implementationData } };
      return;
    }

    _actorDataDictionary.Add(namedActorType, new ActorDataEntry(null, new List<ImplementationData>() { implementationData }));
  }

  public void AddActorData(ActorData data)
  {
    INamedTypeSymbol actorType = data.Type;
    if (actorType.IsGenericType)
    {
      GenericActorsAreNotSupported(actorType);
      return;
    }

    INamedTypeSymbol? interfaceType = FindInterfaceType(data);

    CheckInterface(interfaceType, actorType);

    ITypeSymbol? actorIdType = data.GetSpecifiedIdType(in this);
    if (actorIdType is not null && !IsValidIdType(actorIdType))
    {
      InvalidIdType(data.GenericIdTypeAttribute ?? data.IdTypeAttribute!, actorIdType);
    }
    (List<IFieldSymbol> stateFields, Optional<ITypeSymbol?> idType, bool getsIdFromState, bool canBeVirtual) = GetStateAndIdMembers(actorType, actorIdType);

    List<StateDependencyDescriptor>? stateDependencies = CheckConstructor(actorType, stateFields);

    bool isVirtual = CheckVirtualActor(data, canBeVirtual, stateFields);
    CheckMethods(actorType);

    _ = _actorDataDictionary.TryGetValue(actorType, out ActorDataEntry entry);
    (FactoryData? factory, List<ImplementationData>? implementations) = entry;
    if (factory is not null && idType.HasValue)
    {
      CheckActorFactory(factory, interfaceType, idType.Value, stateFields);
      return;
    }
    else if (_disableFactoryGeneration)
    {
      MissingActorFactoryType(actorType);
    }

    CheckImplementations(actorType, implementations);

    if (_disableFactoryGeneration || !idType.HasValue || interfaceType is null || stateDependencies is null || interfaceType.IsGenericType)
      return;

    CheckActorMessages(actorType, idType.Value);

    _descriptors.Add(new ActorDescriptor(
      actorType.ContainingNamespace.ToDisplayString(Format.FullName),
      actorType.Name,
      interfaceType.ToDisplayString(Format.GlobalFullName),
      idType.Value?.ToDisplayString(Format.GlobalFullName) ?? "string",
      isVirtual,
      getsIdFromState,
      new RecordList<StateDependencyDescriptor>(stateDependencies)));
  }

  private void CheckInterface(INamedTypeSymbol? interfaceType, INamedTypeSymbol actorType)
  {
    if (_disableDiagnostics || interfaceType is null)
      return;

    if (interfaceType.IsGenericType)
    {
      GenericActorsAreNotSupported(interfaceType);
    }

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

      IMethodSymbol serviceMethod = (IMethodSymbol)interfaceMember;

      if (serviceMethod.MethodKind is not MethodKind.Ordinary)
        continue;

      if (actorType.FindImplementationForInterfaceMember(serviceMethod) is not IMethodSymbol implementationMethod)
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

  private INamedTypeSymbol? FindInterfaceType(ActorData data)
  {
    INamedTypeSymbol actorType = data.Type;
    ImmutableArray<INamedTypeSymbol> interfaceTypes = actorType.AllInterfaces;

    INamedTypeSymbol? specifiedInterfaceType = data.GetSpecifiedInterfaceType(in this);

    if (specifiedInterfaceType is null)
    {
      if (interfaceTypes.Length == 0)
      {
        MissingActorInterface(actorType);
        return null;
      }

      INamedTypeSymbol? interfaceType = null;
      foreach (INamedTypeSymbol candidateInterfaceType in interfaceTypes)
      {
        if (candidateInterfaceType.ContainingNamespace.IsCodeArchitectsPlatformMessagingNamespace() && candidateInterfaceType.Name == "IMessageHandler")
          continue;

        if (interfaceType is not null)
        {
          AmbiguousActorInterface(actorType);
          return null;
        }

        interfaceType = candidateInterfaceType;
      }

      return interfaceType;
    }
    else
    {
      foreach (INamedTypeSymbol candidateInterfaceType in interfaceTypes)
      {
        if (SymbolEqualityComparer.Default.Equals(candidateInterfaceType, specifiedInterfaceType))
        {
          return candidateInterfaceType;
        }
      }

      InterfaceNotImplemented(actorType);
      return null;
    }
  }

  private List<StateDependencyDescriptor>? CheckConstructor(INamedTypeSymbol actorType, List<IFieldSymbol> stateFields)
  {
    if (!TryGetConstructor(actorType, out IMethodSymbol? constructor))
      return null;

    int stateArity = stateFields.Count;
    int index = 0;
    List<StateDependencyDescriptor> stateDependencies = new(stateArity);
    bool stateComponentMismatchRaised = false;

    foreach (IParameterSymbol parameter in constructor.Parameters)
    {
      if (IsStateParameter(constructor, parameter, stateFields, ref stateComponentMismatchRaised))
      {
        stateDependencies.Add(new StateDependencyDescriptor(
          parameter.Type.ToDisplayString(Format.GlobalFullName),
          parameter.Name));

        index++;
        continue;
      }

      CheckActorContext(actorType, parameter);
    }

    if (!stateComponentMismatchRaised && index != stateArity)
    {
      StateComponentNameMismatch(constructor);
    }

    return stateDependencies;
  }

  private bool IsStateParameter(IMethodSymbol constructor, IParameterSymbol parameter, List<IFieldSymbol> stateFields, ref bool stateComponentMismatchRaised)
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
          if (!stateComponentMismatchRaised)
          {
            stateComponentMismatchRaised = true;
            StateComponentNameMismatch(constructor);
          }
          break;
        }

        found = true;
      }
    }

    return found;
  }

  private void CheckActorContext(INamedTypeSymbol actorType, IParameterSymbol parameter)
  {
    if (_disableDiagnostics)
      return;

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
      if (HasActorConstructorAttribute(candidateConstructor))
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

    static bool HasActorConstructorAttribute(IMethodSymbol constructor)
    {
      foreach (AttributeData attribute in constructor.GetAttributes())
      {
        if (attribute.IsActorConstructorAttribute())
          return true;
      }

      return false;
    }
  }

  private (List<IFieldSymbol> StateFields, Optional<ITypeSymbol?> IdType, bool GetsIdFromState, bool CanBeVirtual) GetStateAndIdMembers(INamedTypeSymbol actorType, ITypeSymbol? actorIdType)
  {
    List<IFieldSymbol> stateFields = new();
    ISymbol? idMember = null;
    ITypeSymbol? idType = null;
    bool ambiguousActorIdSource = false;
    bool canBeVirtual = true;
    bool getsIdFromState = false;

    foreach (ISymbol member in actorType.GetMembers())
    {
      if (member is not IFieldSymbol field)
        continue;

      ISymbol candidateMember;
      if (field.AssociatedSymbol is ISymbol associatedSymbol)
      {
        if (associatedSymbol.Kind is not SymbolKind.Property)
          continue;

        candidateMember = associatedSymbol;
      }
      else
      {
        candidateMember = field;
      }

      ITypeSymbol memberType = field.Type;

      AttributeData? stateAttribute = null;
      AttributeData? actorIdAttribute = null;
      foreach (AttributeData attribute in candidateMember.GetAttributes())
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

      bool implementsActorIdSource = false;
      if (memberType.TypeKind is TypeKind.Class && memberType.SpecialType is not SpecialType.System_String)
      {
        foreach (INamedTypeSymbol interfaceType in memberType.AllInterfaces)
        {
          if (interfaceType.IsActorIdSourceType(out idType))
          {
            if (implementsActorIdSource)
            {
              MultipleIdSourceInterfaces(memberType);
              idType = null;
              break;
            }

            implementsActorIdSource = true;
            if (idMember is not null)
            {
              ambiguousActorIdSource = true;
            }
            else
            {
              idMember = candidateMember;
              if (!IsValidIdType(idType))
              {
                InvalidIdType(idMember, idType);
              }
              if (actorIdType is not null && !SymbolEqualityComparer.Default.Equals(idType, actorIdType))
              {
                InvalidIdSource(idMember, idType, actorIdType);
              }

              if (stateAttribute is not null)
              {
                getsIdFromState = true;
              }
            }
          }
        }
      }
      if (!implementsActorIdSource && actorIdAttribute is not null)
      {
        if (idMember is not null)
        {
          ambiguousActorIdSource = true;
        }
        else
        {
          idMember = candidateMember;
          idType = memberType;
          if (!IsValidIdType(idType))
          {
            InvalidIdType(idMember, idType);
          }
          if (actorIdType is not null && !SymbolEqualityComparer.Default.Equals(idType, actorIdType))
          {
            InvalidIdSource(idMember, idType, actorIdType);
          }

          if (stateAttribute is not null)
          {
            getsIdFromState = true;
          }
        }
      }

      if (stateAttribute is not null)
      {
        CheckStateType(field, stateAttribute);
        CheckDefaultValue(field, stateAttribute, ref canBeVirtual);

        stateFields.Add(field);
      }
    }

    if (ambiguousActorIdSource)
    {
      AmbiguousActorIdSource(actorType);
      return (stateFields, default, getsIdFromState, canBeVirtual);
    }

    return (stateFields, new Optional<ITypeSymbol?>(idType ?? actorIdType), getsIdFromState, canBeVirtual);
  }

  private void CheckStateType(IFieldSymbol stateField, AttributeData stateAttribute)
  {
    if (_disableDiagnostics)
      return;

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

  private void CheckDefaultValue(IFieldSymbol stateField, AttributeData stateAttribute, ref bool canBeVirtual)
  {
    if (_disableDiagnostics)
      return;

    if (!HasDefaultValue(stateAttribute, out TypedConstant defaultValue))
    {
      ITypeSymbol memberType = stateField.Type;
      canBeVirtual &= memberType.IsValueType || HasParameterlessConstructor(memberType);
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

    static bool HasDefaultValue(AttributeData stateAttribute, [NotNullWhen(true)] out TypedConstant defaultValue)
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

    static bool HasParameterlessConstructor(ITypeSymbol type)
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
  }

  private bool IsValidIdType(ITypeSymbol idType)
  {
    if (_disableDiagnostics || idType.SpecialType is SpecialType.System_String)
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

  private void CheckActorFactory(FactoryData data, INamedTypeSymbol? interfaceType, ITypeSymbol? idType, List<IFieldSymbol> stateFields)
  {
    if (_disableDiagnostics || data is null || interfaceType is null)
      return;

    INamedTypeSymbol factoryType = data.Type;
    int stateArity = stateFields.Count;

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

    if (getMethod is null || stateArity != 0 && createAsyncMethod is null)
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

  private bool CheckVirtualActor(ActorData data, bool canBeVirtual, List<IFieldSymbol> stateFields)
  {
    bool isExplicitVirtual = data.VirtualAttribute is not null;

    if (!canBeVirtual && isExplicitVirtual)
    {
      ActorCannotBeVirtual(data.Type, data.VirtualAttribute!);
    }

    return isExplicitVirtual || stateFields.Count == 0;
  }

  private void CheckMethods(INamedTypeSymbol implementationType)
  {
    if (_disableDiagnostics)
      return;

    foreach (ISymbol member in implementationType.GetMembers())
    {
      if (member is not IMethodSymbol method || method.MethodKind is not MethodKind.Ordinary || method.IsOverride)
        continue;

      ITypeSymbol returnType = method.ReturnType;
      if (!returnType.IsTaskLikeType() && returnType.SpecialType is not SpecialType.System_Void)
        continue;

      CheckCancellationTokenParameter(method);
    }
  }

  private void CheckCancellationTokenParameter(IMethodSymbol implementationMethod)
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

  private void CheckImplementations(INamedTypeSymbol actorType, List<ImplementationData>? implementations)
  {
    if (_disableDiagnostics)
      return;

    if (implementations is null || implementations.Count == 0)
    {
      if (actorType.IsAbstract)
      {
        AbstractImplementation(actorType);
      }
      return;
    }

    bool hasDefaultImplementation = false;
    bool multipleDefaultImplementationsRaised = false;
    foreach (ImplementationData implementationData in implementations)
    {
      INamedTypeSymbol implementationType = implementationData.Type;

      foreach (ISymbol member in implementationType.GetMembers())
      {
        if (member.Kind is not SymbolKind.Field)
          continue;

        foreach (AttributeData attribute in member.GetAttributes())
        {
          if (attribute.AttributeClass is not INamedTypeSymbol attributeType)
            continue;

          if (!attributeType.ContainingNamespace.IsCodeArchitectsPlatformActorsNamespace() || attributeType.Name is not "StateAttribute")
            continue;

          StateMustBeDefinedInBaseActor(implementationType, attribute);
          break;
        }
      }

      CheckMethods(implementationType);

      AttributeData implementationAttribute = implementationData.ImplementationAttribute ?? implementationData.GenericImplementationAttribute!;

      foreach (KeyValuePair<string, TypedConstant> arg in implementationAttribute.NamedArguments)
      {
        if (arg.Key == "IsDefault" && arg.Value.Value is bool isDefault && isDefault)
        {
          if (hasDefaultImplementation && !multipleDefaultImplementationsRaised)
          {
            multipleDefaultImplementationsRaised = true;
            MultipleDefaultImplementations(actorType);
          }

          hasDefaultImplementation = true;
        }
      }
    }
  }

  private void CheckActorMessages(INamedTypeSymbol actorType, ITypeSymbol? idType)
  {
    if (_disableDiagnostics)
      return;

    foreach (INamedTypeSymbol interfaceType in actorType.AllInterfaces)
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
        InvalidActorMessage(actorType, messageType);
        continue;
      }

      ITypeSymbol messageIdType = actorMessageInterfaceType.TypeArguments[0];

      if (idType is null)
      {
        if (messageIdType.SpecialType is not SpecialType.System_String)
        {
          InvalidActorMessage(actorType, messageType);
          continue;
        }
      }
      else
      {
        if (!SymbolEqualityComparer.Default.Equals(messageIdType, idType))
        {
          InvalidActorMessage(actorType, messageType);
          continue;
        }
      }
    }
  }


  private readonly record struct ActorDataEntry(FactoryData? Factory, List<ImplementationData>? Implementations);
}
