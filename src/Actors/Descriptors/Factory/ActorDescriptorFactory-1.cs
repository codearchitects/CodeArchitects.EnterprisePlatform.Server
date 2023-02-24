using CodeArchitects.Platform.Actors.Descriptors.Implementation;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Messaging;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Common.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal abstract class ActorDescriptorFactory<TActor> : ActorDescriptorFactory
  where TActor : class
{
  private static readonly MethodInfo s_createDescriptorMethod = typeof(ActorDescriptorFactory<TActor>).GetRequiredMethod(
    name: nameof(CreateDescriptor),
    bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
    types: new[] { typeof(bool) });

  private readonly IStateTypeBuilder _stateTypeBuilder;
  private readonly IActivityTypeBuilder _activityTypeBuilder;

  public ActorDescriptorFactory(IStateTypeBuilder stateTypeBuilder, IActivityTypeBuilder activityTypeBuilder)
  {
    _stateTypeBuilder = stateTypeBuilder;
    _activityTypeBuilder = activityTypeBuilder;
  }

  protected Type ActorType => typeof(TActor);

  protected abstract Type? InterfaceType { get; }

  protected abstract Type? FactoryType { get; }

  protected abstract bool IsExplicitVirtual { get; }

  protected abstract IReadOnlyCollection<StateComponentMetadata<TActor>> StateComponents { get; }

  protected abstract ImplementationDescriptorFactory<TActor> BaseImplementationFactory { get; }

  protected abstract IReadOnlyCollection<ImplementationDescriptorFactory<TActor>> ImplementationFactories { get; }

  public int StateComponentCount => StateComponents.Count;

  protected abstract IReadOnlyCollection<IMessageHandlerMetadata> GetMessageHandlerMetadataCollection(IMethodDescriptor activity);

  public bool TryGetStateComponent(string name, [NotNullWhen(true)] out StateComponentMetadata<TActor>? stateComponent)
  {
    stateComponent = null;
    
    foreach (StateComponentMetadata<TActor> component in StateComponents)
    {
      string memberName = component.Member.Name;

      bool isMatch =
        name.MatchesUnderscorePrefixConvention(memberName) || // _name
        name == memberName                                 || // name
        memberName.MatchesCamelCaseConvention(name)        || // Name
        name.MatchesMemberPrefixConvention(memberName);       // m_name

      if (isMatch)
      {
        if (stateComponent is not null)
          throw InvalidActorException.StateComponentsMismatch(typeof(TActor));

        stateComponent = component;
      }
    }

    return stateComponent is not null;
  }

  public override IActorDescriptor CreateDescriptor()
  {
    if (ActorType.IsGenericType)
      throw InvalidActorException.GenericActorsAreNotSupported(ActorType);

    bool isPolymorphic = ImplementationFactories.Count > 0;
    Type stateType = _stateTypeBuilder.Build(ActorType, StateComponents, isPolymorphic);

    return (IActorDescriptor)s_createDescriptorMethod.MakeGenericMethod(stateType.UnderlyingSystemType).Invoke(this, new object?[] { isPolymorphic })!;
  }

  private IActorDescriptor<TActor, TState> CreateDescriptor<TState>(bool isPolymorphic)
    where TState : ActorState, new()
  {
    Type interfaceType = GetInterfaceType();
    CheckInterfaceType(interfaceType);

    bool isStateless = StateComponents.Count == 0;
    IStateDescriptor<TState> state = CreateState<TState>(isStateless);
    Action<TActor, TState> updateState = CreateUpdateStateFunc<TState>(state.Fields);
    IActorIdDescriptor<TState> id = CreateId<TState>(state.Fields);
    bool isVirtual = IsExplicitVirtual || isStateless;
    IActorFactoryDescriptor factory = CreateFactory(interfaceType, id, isVirtual);
    Type activityBaseType = _activityTypeBuilder.BuildBase(ActorType);
    MethodDescriptorFactory methodDescriptorFactory = MethodDescriptorFactory.Create(_activityTypeBuilder, ActorType, interfaceType, activityBaseType);
    IReadOnlyCollection<IMessageHandlerDescriptor> messageHandlers = CreateMessageHandlers(methodDescriptorFactory, id.Type);

    if (!isPolymorphic)
    {
      return new OrdinaryActorDescriptor<TActor, TState>(
        interfaceType,
        isVirtual,
        activityBaseType,
        updateState,
        state,
        id,
        factory,
        BaseImplementationFactory.CreateDescriptor<TState>(state.Fields),
        methodDescriptorFactory.Methods,
        methodDescriptorFactory.Activities,
        messageHandlers);
    }

    PolymorphicActorDescriptor<TActor, TState> polymorphicActor = new PolymorphicActorDescriptor<TActor, TState>(
      interfaceType,
      isVirtual,
      activityBaseType,
      updateState,
      state,
      id,
      factory,
      new AbstractBaseImplementationDescriptor<TActor, TState>(),
      methodDescriptorFactory.Methods,
      methodDescriptorFactory.Activities,
      messageHandlers);

    foreach (ImplementationDescriptorFactory<TActor> implementationFactory in ImplementationFactories)
    {
      methodDescriptorFactory.AddImplementationType(implementationFactory.ImplementationType);
      IImplementationDescriptor<TActor, TState> implementation = implementationFactory.CreateDescriptor<TState>(state.Fields);
      polymorphicActor.AddImplementation(implementation, implementationFactory.IsDefault);
    }

    return polymorphicActor;
  }

  private Type GetInterfaceType()
  {
    Type[] implementedInterfaceTypes = ActorType.GetInterfaces();

    if (InterfaceType is null)
    {
      if (implementedInterfaceTypes.Length == 0)
        throw InvalidActorException.MissingActorInterface(ActorType);

      Type? interfaceType = null;
      foreach (Type implementedInterfaceType in implementedInterfaceTypes)
      {
        if (MessagingMetadata.Metadata.IsMessageHandlerType(implementedInterfaceType))
          continue;

        if (interfaceType is not null)
          throw InvalidActorException.AmbiguousActorInterface(ActorType);

        interfaceType = implementedInterfaceType;
      }

      return interfaceType!;
    }

    if (!InterfaceType.IsInterface)
      throw InvalidActorException.InterfaceTypeIsNotAnInterface(ActorType, InterfaceType);

    foreach (Type implementedInterfaceType in implementedInterfaceTypes)
    {
      if (InterfaceType == implementedInterfaceType)
        return implementedInterfaceType;
    }

    throw InvalidActorException.InterfaceNotImplemented(ActorType);
  }

  private void CheckInterfaceType(Type interfaceType)
  {
    if (interfaceType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Length != 0)
      throw InvalidActorException.PropertiesAreNotSupported(ActorType, interfaceType);

    if (interfaceType.GetEvents(BindingFlags.Instance | BindingFlags.Public).Length != 0)
      throw InvalidActorException.EventsAreNotSupported(ActorType, interfaceType);
  }

  private Action<TActor, TState> CreateUpdateStateFunc<TState>(IReadOnlyList<FieldInfo> stateTypeFields)
    where TState : ActorState
  {
    ParameterExpression actorParam = Expression.Parameter(typeof(TActor), "actor");
    ParameterExpression stateParam = Expression.Parameter(typeof(TState), "state");

    List<Expression> statements = new();
    foreach (StateComponentMetadata<TActor> component in StateComponents)
    {
      FieldInfo stateField = stateTypeFields[component.Index];

      statements.Add(Expression.Assign(
        left: Expression.Field(stateParam, stateField),
        right: Expression.PropertyOrField(actorParam, component.Member.Name)));
    }

    return Expression.Lambda<Action<TActor, TState>>(
      body: Expression.Block(statements),
      parameters: new[] { actorParam, stateParam })
      .Compile();
  }

  private IStateDescriptor<TState> CreateState<TState>(bool isStateless)
    where TState : ActorState, new()
  {
    StateDescriptor<TState> state = new();

    if (isStateless)
    {
      state.DefaultValue = new();
    }
    else if (IsExplicitVirtual)
    {
      TState defaultStateValue = new();
      IReadOnlyList<FieldInfo> stateTypeFields = state.Fields;

      foreach (StateComponentMetadata<TActor> component in StateComponents)
      {
        FieldInfo stateField = stateTypeFields[component.Index];

        if (!component.HasDefaultValue(out object? defaultComponentValue))
        {
          try
          {
            defaultComponentValue = Activator.CreateInstance(stateField.FieldType);
          }
          catch
          {
            throw InvalidActorException.ActorCannotBeVirtual(ActorType);
          }
        }
        else
        {
          if (!component.Type.IsInstanceOfType(defaultComponentValue))
            throw InvalidActorException.InvalidDefaultValue(ActorType, component.Member);
        }

        stateField.SetValue(defaultStateValue, defaultComponentValue);
      }

      state.DefaultValue = defaultStateValue;
    }

    return state;
  }

  public IActorIdDescriptor<TState> CreateId<TState>(IReadOnlyList<FieldInfo> stateFields)
    where TState : ActorState
  {
    IActorIdDescriptor<TState>? id = null;

    ParameterExpression stateParam = Expression.Parameter(typeof(TState), "state");
    ParameterExpression idParam = Expression.Parameter(typeof(string), "id");

    foreach (StateComponentMetadata<TActor> component in StateComponents)
    {
      int stateIndex = component.Index;

      if (component.IsActorIdSource(out Type? idType))
      {
        if (id is not null)
          throw InvalidActorException.AmbiguousActorIdSource(ActorType);

        Type actorIdSourceType = typeof(IActorIdSource<>).MakeGenericType(idType.UnderlyingSystemType);
        InterfaceMapping mapping = component.Type.GetInterfaceMap(actorIdSourceType);

        MethodInfo getActorIdMethod = mapping.TargetMethods.Single(method => method.Name.Contains(nameof(IActorIdSource<object>.GetActorId)));
        MethodInfo setActorIdMethod = mapping.TargetMethods.Single(method => method.Name.Contains(nameof(IActorIdSource<object>.SetActorId)));

        Expression parseIdExpression = GetParseIdExpression(idParam, idType);
        
        Action<TState, string> setId = Expression.Lambda<Action<TState, string>>(
          body: Expression.Call(
            instance: Expression.Field(
              expression: stateParam,
              field: stateFields[stateIndex]),
            method: setActorIdMethod,
            arguments: parseIdExpression),
          parameters: new[] { stateParam, idParam })
          .Compile();

        return new SourceActorIdDescriptor<TState>(getActorIdMethod, stateIndex, setId);
      }
      else if (component.IsActorId)
      {
        if (id is not null)
          throw InvalidActorException.AmbiguousActorIdSource(ActorType);

        Expression parseIdExpression = GetParseIdExpression(idParam, component.Type);

        Action<TState, string> setId = Expression.Lambda<Action<TState, string>>(
          body: Expression.Assign(
            left: Expression.Field(
              expression: stateParam,
              field: stateFields[stateIndex]),
            right: parseIdExpression),
          parameters: new[] { stateParam, idParam })
          .Compile();

        return new ComponentActorIdDescriptor<TState>(component.Type, stateIndex, setId);
      }
    }

    return id ?? DefaultActorIdDescriptor<TState>.Instance;
  }

  private Expression GetParseIdExpression(ParameterExpression idParam, Type idType)
  {
    MethodInfo? parseMethod = idType.GetMethod(
      name: "Parse",
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      binder: null,
      types: new[] { typeof(string) },
      modifiers: null);

    if (parseMethod is not null && parseMethod.ReturnType == idType)
    {
      return Expression.Call(
        method: parseMethod,
        arg0: idParam);
    }

    parseMethod = idType.GetMethod(
      name: "Parse",
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      binder: null,
      types: new[] { typeof(string), typeof(IFormatProvider) },
      modifiers: null);

    if (parseMethod is not null && parseMethod.ReturnType == idType)
    {
      return Expression.Call(
        method: parseMethod,
        arg0: idParam,
        arg1: Expression.Constant(null, typeof(IFormatProvider)));
    }

    throw InvalidActorException.InvalidIdType(ActorType, idType);
  }

  private ActorFactoryDescriptor CreateFactory(Type interfaceType, IActorIdDescriptor id, bool isVirtual)
  {
    if (FactoryType is not Type factoryType)
      throw InvalidActorException.MissingActorFactoryType(ActorType);

    int expectedMethodCount = isVirtual ? 1 : 2;

    if (factoryType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Length != expectedMethodCount)
      throw InvalidActorException.InvalidActorFactoryType(ActorType, factoryType);

    if (factoryType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Length != 0)
      throw InvalidActorException.InvalidActorFactoryType(ActorType, factoryType);

    if (factoryType.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Length != 0)
      throw InvalidActorException.InvalidActorFactoryType(ActorType, factoryType);

    MethodInfo? createAsyncMethod;

    if (isVirtual)
    {
      createAsyncMethod = null;
    }
    else
    {
      Type[] createAsyncMethodTypes = (id.HasIdSource ? Type.EmptyTypes : new[] { typeof(string) })
        .Concat(StateComponents.Select(component => component.Type))
        .Concat(new[] { typeof(CancellationToken) })
        .ToArray();

      createAsyncMethod = factoryType.GetMethod(
        name: "CreateAsync",
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        binder: null,
        types: createAsyncMethodTypes,
        modifiers: null);

      if (createAsyncMethod is null || createAsyncMethod.ReturnType != typeof(Task<>).MakeGenericType(interfaceType))
        throw InvalidActorException.InvalidActorFactoryType(ActorType, factoryType);
    }

    MethodInfo? getMethod = factoryType.GetMethod(
      name: "Get",
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      binder: null,
      types: new[] { id.Type.UnderlyingSystemType },
      modifiers: null);

    if (getMethod is null || getMethod.ReturnType != interfaceType)
      throw InvalidActorException.InvalidActorFactoryType(ActorType, factoryType);

    return new ActorFactoryDescriptor(factoryType, createAsyncMethod, getMethod);
  }

  private IReadOnlyCollection<MessageHandlerDescriptor> CreateMessageHandlers(MethodDescriptorFactory methodDescriptorFactory, Type idType)
  {
    List<MessageHandlerDescriptor> result = new();

    foreach (Type interfaceType in ActorType.GetInterfaces())
    {
      if (MessagingMetadata.Metadata.IsMessageHandlerType(interfaceType))
      {
        InterfaceMapping interfaceMapping = ActorType.GetInterfaceMap(interfaceType);
        MethodInfo implementationMethod = interfaceMapping.TargetMethods[0];
        IMethodDescriptor activity = methodDescriptorFactory.GetActivity(implementationMethod);

        Type[] typeArguments = interfaceType.GetGenericArguments();
        Type messageType = typeArguments[0];
        Type resultType = typeArguments.Length > 1 ? typeArguments[1] : typeof(void);

        if (!typeof(IActorMessage<>).MakeGenericType(idType).IsAssignableFrom(messageType))
          throw InvalidActorException.InvalidActorMessage(ActorType, messageType);

        IReadOnlyCollection<IMessageHandlerMetadata> handlerMetadataCollection = GetMessageHandlerMetadataCollection(activity);

        result.Add(new MessageHandlerDescriptor(interfaceType, messageType, resultType, activity, handlerMetadataCollection));
      }
    }

    return result;
  }
}
