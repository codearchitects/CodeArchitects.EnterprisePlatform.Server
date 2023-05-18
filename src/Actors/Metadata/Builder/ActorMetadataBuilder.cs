using CodeArchitects.Platform.Actors.Metadata.Factory;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Common.Exceptions;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

internal class ActorMetadataBuilder<TActor> : ActorDescriptorFactory<TActor>, IActorMetadataBuilder<TActor>
  where TActor : class
{
  private readonly Dictionary<MemberInfo, StateComponentMetadata<TActor>> _stateComponents;
  private readonly ImplementationMetadataBuilder<TActor, TActor> _baseImplementationBuilder;
  private readonly Dictionary<Type, ImplementationDescriptorFactory<TActor>> _implementations;
  private readonly Dictionary<Type, List<IMessageHandlerMetadata>> _handlerMetadataDictionary;
  private readonly HashSet<MemberMetadata> _actorIdMembers;
  private Type? _interfaceType;
  private Type? _idType;
  private Type? _factoryType;
  private bool _isExplicitVirtual;

  public ActorMetadataBuilder(IStateTypeBuilder stateTypeBuilder, IActivityTypeBuilder activityTypeBuilder)
    : base(stateTypeBuilder, activityTypeBuilder)
  {
    _baseImplementationBuilder = new ImplementationMetadataBuilder<TActor, TActor>(0, this);
    _stateComponents = new();
    _implementations = new();
    _handlerMetadataDictionary = new();
    _actorIdMembers = new();
  }

  protected override Type? InterfaceType => _interfaceType;

  protected override Type? FactoryType => _factoryType;

  protected override Type? IdType => _idType;

  protected override ImplementationDescriptorFactory<TActor> BaseImplementationFactory => _baseImplementationBuilder;

  protected override IReadOnlyCollection<ImplementationDescriptorFactory<TActor>> ImplementationFactories => _implementations.Values;

  protected override bool IsExplicitVirtual => _isExplicitVirtual;

  protected override IReadOnlyCollection<StateComponentMetadata<TActor>> StateComponents => _stateComponents.Values;

  protected override IEnumerable<MemberMetadata> ActorIdMembers => _actorIdMembers.Where(metadata => !_stateComponents.ContainsKey(metadata.Member));

  public IActorMetadataBuilder<TActor> AsBuilder() => this;

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasInterfaceType(Type interfaceType)
  {
    if (interfaceType is null)
      throw new ArgumentNullException(nameof(interfaceType));

    HasInterfaceTypeCore(interfaceType);
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasInterfaceType<TInterface>()
    where TInterface : class
  {
    HasInterfaceTypeCore(typeof(TInterface));
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasFactoryType(Type factoryType)
  {
    if (factoryType is null)
      throw new ArgumentNullException(nameof(factoryType));

    HasFactoryTypeCore(factoryType);
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasFactoryType<TFactory>()
    where TFactory : class
  {
    HasFactoryTypeCore(typeof(TFactory));
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.IsVirtual(bool isVirtual)
  {
    _isExplicitVirtual = isVirtual;
    return this;
  }

  private void HasInterfaceTypeCore(Type interfaceType)
  {
    if (!interfaceType.IsInterface)
      throw new ArgumentException("The interface type must be an interface.", nameof(interfaceType));

    _interfaceType = interfaceType;
  }

  private void HasFactoryTypeCore(Type factoryType)
  {
    if (!factoryType.IsInterface)
      throw new ArgumentException("The factory type must be an interface.", nameof(factoryType));

    _factoryType = factoryType;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasState<TState>(Expression<Func<TActor, TState>> memberExpression)
  {
    if (memberExpression is null)
      throw new ArgumentNullException(nameof(memberExpression));
    if (memberExpression.Body is not MemberExpression body || body.Expression != memberExpression.Parameters[0])
      throw new ArgumentException("The expression must represent a field or property access.", nameof(memberExpression));

    HasStateCore<TState>(body.Member, body.Type, _ => { });
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasState<TState>(Expression<Func<TActor, TState>> memberExpression, Action<IStateComponentBuilder<TActor, TState>> configure)
  {
    if (memberExpression is null)
      throw new ArgumentNullException(nameof(memberExpression));
    if (memberExpression.Body is not MemberExpression body || body.Expression != memberExpression.Parameters[0])
      throw new ArgumentException("The expression must represent a field or property access.", nameof(memberExpression));

    HasStateCore(body.Member, body.Type, configure);
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasState(string memberName)
  {
    if (memberName is null)
      throw new ArgumentNullException(nameof(memberName));

    FieldInfo? field = typeof(TActor).GetField(
      name: memberName,
      bindingAttr: BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    if (field is not null)
    {
      HasStateCore<object>(field, field.FieldType, _ => { });
      return this;
    }

    PropertyInfo? property = typeof(TActor).GetProperty(
      name: memberName,
      bindingAttr: BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    if (property is not null)
    {
      HasStateCore<object>(property, property.PropertyType, _ => { });
      return this;
    }

    throw new MissingMemberException(typeof(TActor).Name, memberName);
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasState<TState>(string memberName, Action<IStateComponentBuilder<TActor, TState>> configure)
  {
    if (memberName is null)
      throw new ArgumentNullException(nameof(memberName));

    FieldInfo? field = typeof(TActor).GetField(
      name: memberName,
      bindingAttr: BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    if (field is not null)
    {
      if (typeof(TState) != field.FieldType)
        throw new TypeArgumentException($"'{nameof(TState)}' does not match the member type.", nameof(TState));

      HasStateCore(field, field.FieldType, configure);
      return this;
    }

    PropertyInfo? property = typeof(TActor).GetProperty(
      name: memberName,
      bindingAttr: BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    if (property is not null)
    {
      if (typeof(TState) != property.PropertyType)
        throw new TypeArgumentException($"'{nameof(TState)}' does not match the member type.", nameof(TState));

      HasStateCore(property, property.PropertyType, configure);
      return this;
    }

    throw new MissingMemberException(typeof(TActor).Name, memberName);
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasId<TActorId>(Expression<Func<TActor, TActorId>> memberExpression)
  {
    if (memberExpression is null)
      throw new ArgumentNullException(nameof(memberExpression));
    if (memberExpression.Body is not MemberExpression body || body.Expression != memberExpression.Parameters[0])
      throw new ArgumentException("The expression must represent a field or property access.", nameof(memberExpression));

    _actorIdMembers.Add(new MemberMetadata(body.Member, body.Type));
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasId(string memberName)
  {
    if (memberName is null)
      throw new ArgumentNullException(nameof(memberName));

    FieldInfo? field = typeof(TActor).GetField(
      name: memberName,
      bindingAttr: BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    if (field is not null)
    {
      _actorIdMembers.Add(new MemberMetadata(field, field.FieldType));
      return this;
    }

    PropertyInfo? property = typeof(TActor).GetProperty(
      name: memberName,
      bindingAttr: BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    if (property is not null)
    {
      _actorIdMembers.Add(new MemberMetadata(property, property.PropertyType));
      return this;
    }

    throw new MissingMemberException(typeof(TActor).Name, memberName);
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasConstructor(ConstructorInfo constructor)
  {
    if (constructor is null)
      throw new ArgumentNullException(nameof(constructor));

    _baseImplementationBuilder.AsBuilder().HasConstructor(constructor);
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasConstructor(params Type[] parameterTypes)
  {
    if (parameterTypes is null)
      throw new ArgumentNullException(nameof(parameterTypes));

    _baseImplementationBuilder.AsBuilder().HasConstructor(parameterTypes);
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasConstructor(Expression<Func<IConstructorArgument<TActor>, TActor>> constructorExpression)
  {
    if (constructorExpression is null)
      throw new ArgumentNullException(nameof(constructorExpression));

    _baseImplementationBuilder.AsBuilder().HasConstructor(constructorExpression);
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasImplementation<TImplementation>()
  {
    return HasImplementationCore<TImplementation>(_ => { });
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasImplementation<TImplementation>(Action<IImplementationMetadataBuilder<TActor, TImplementation>> configure)
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    return HasImplementationCore(configure);
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasIdType(Type actorIdType)
  {
    if (actorIdType is null)
      throw new ArgumentNullException(nameof(actorIdType));

    _idType = actorIdType;
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasIdType<TActorId>()
  {
    _idType = typeof(TActorId);
    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.IsMessageHandler(Type messageType, Action<IMessageHandlerMetadataBuilder> configure)
  {
    if (messageType is null)
      throw new ArgumentNullException(nameof(messageType));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    MessageHandlerMetadataBuilder builder = new();
    configure(builder);

    if (!_handlerMetadataDictionary.TryGetValue(messageType, out List<IMessageHandlerMetadata>? metadataCollection))
    {
      metadataCollection = new();
      _handlerMetadataDictionary.Add(messageType, metadataCollection);
    }

    metadataCollection.Add(builder.Build());
    return this;
  }

  protected override IReadOnlyCollection<IMessageHandlerMetadata> GetMessageHandlerMetadataCollection(IMethodDescriptor activity)
  {
    if (_handlerMetadataDictionary.TryGetValue(activity.ParameterTypes[0], out List<IMessageHandlerMetadata>? metadataCollection))
      return metadataCollection;

    return Array.Empty<IMessageHandlerMetadata>();
  }

  private void HasStateCore<TState>(MemberInfo member, Type type, Action<IStateComponentBuilder<TActor, TState>> configure)
  {
    StateComponentBuilder<TActor, TState> builder;
    if (_stateComponents.TryGetValue(member, out var metadata))
    {
      builder = (StateComponentBuilder<TActor, TState>)metadata;
    }
    else
    {
      builder = new(_stateComponents.Count, member, type);
      _stateComponents.Add(member, builder);
    }

    configure(builder);
  }

  private IActorMetadataBuilder<TActor> HasImplementationCore<TImplementation>(Action<IImplementationMetadataBuilder<TActor, TImplementation>> configure)
    where TImplementation : TActor
  {
    Type implementationType = typeof(TImplementation);
    if (implementationType.IsAbstract)
      throw new TypeArgumentException("An abstract class cannot be an actor implementation.", nameof(TImplementation));

    ImplementationMetadataBuilder<TActor, TImplementation> builder;
    if (_implementations.TryGetValue(implementationType, out var factory))
    {
      builder = (ImplementationMetadataBuilder<TActor, TImplementation>)factory;
    }
    else
    {
      builder = new(_implementations.Count + 1, this);
      _implementations.Add(implementationType, builder);
    }

    configure(builder);
    return this;
  }
}
