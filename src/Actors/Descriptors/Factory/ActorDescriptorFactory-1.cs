using CodeArchitects.Platform.Actors.Descriptors.Implementation;
using CodeArchitects.Platform.Actors.Infrastructure;
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
    IActorIdDescriptor id = CreateId();
    bool isVirtual = IsExplicitVirtual || isStateless;
    IActorFactoryDescriptor factory = CreateFactory(interfaceType, id, isVirtual);
    Type activityBaseType = _activityTypeBuilder.BuildBase(ActorType);
    MethodDescriptorFactory methodDescriptorFactory = MethodDescriptorFactory.Create(_activityTypeBuilder, ActorType, interfaceType, activityBaseType);

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
        methodDescriptorFactory.Activities);
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
      methodDescriptorFactory.Activities);

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

      if (implementedInterfaceTypes.Length != 1)
        throw InvalidActorException.AmbiguousActorInterface(ActorType);

      return implementedInterfaceTypes[0];
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

  public IActorIdDescriptor CreateId()
  {
    IActorIdDescriptor? id = null;

    foreach (StateComponentMetadata<TActor> component in StateComponents)
    {
      if (!component.IsActorIdSource(out PropertyInfo? actorIdProperty))
        continue;

      if (id is not null)
        throw InvalidActorException.AmbiguousActorIdSource(ActorType);

      id = actorIdProperty is null
        ? new ComponentActorIdDescriptor(component.Type, component.Index)
        : new PropertyActorIdDescriptor(actorIdProperty, component.Index);
    }

    return id ?? DefaultActorIdDescriptor.Instance;
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
      types: new[] { id.Type },
      modifiers: null);

    if (getMethod is null || getMethod.ReturnType != interfaceType)
      throw InvalidActorException.InvalidActorFactoryType(ActorType, factoryType);

    return new ActorFactoryDescriptor(factoryType, createAsyncMethod, getMethod);
  }
}
