using CodeArchitects.Platform.Actors.Metadata.Implementation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

internal class ActorMetadataBuilder<TActor> : ActorMetadata, IActorMetadataBuilder<TActor>
  where TActor : class
{
  private readonly ImplementationMetadataBuilder<TActor> _baseImplementation;
  private Type? _interfaceType;
  private bool _isExplicitVirtual;

  public ActorMetadataBuilder()
  {
    _baseImplementation = new(typeof(TActor), AddStateField);
  }

  public override Type? InterfaceType => _interfaceType;

  public override bool IsExplicitVirtual => _isExplicitVirtual;

  public override IImplementationMetadata BaseImplementation => _baseImplementation;

  public IActorMetadataBuilder<TActor> AsBuilder() => this;

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasFactoryType(Type factoryType)
  {
    if (factoryType is null)
      throw new ArgumentNullException(nameof(factoryType));
    
    return HasFactoryTypeCore(factoryType);
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasFactoryType<TFactory>()
    where TFactory : class
  {
    return HasFactoryTypeCore(typeof(TFactory));
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasInterfaceType(Type interfaceType)
  {
    if (interfaceType is null)
      throw new ArgumentNullException(nameof(interfaceType));

    return HasInterfaceTypeCore(interfaceType);
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasInterfaceType<TInterface>()
    where TInterface : class
  {
    return HasInterfaceTypeCore(typeof(TInterface));
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.IsVirtual(bool isVirtual = true)
  {
    _isExplicitVirtual = isVirtual;

    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasActorConstructor(Expression<Func<IConstructorArgumentSpec, TActor>> constructorExpression)
  {
    _baseImplementation.AsBuilder().HasActorConstructor(constructorExpression);

    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasMethod<TReturn>(Expression<Func<TActor, IMethodArgumentSpec, TReturn>> methodExpression, Action<IMethodMetadataBuilder> configure)
  {
    _baseImplementation.AsBuilder().HasMethod(methodExpression, configure);

    return this;
  }

  IActorMetadataBuilder<TActor> IActorMetadataBuilder<TActor>.HasImplementation<TImplementation>(Action<IImplementationMetadataBuilder<TImplementation>> configure)
  {
    ImplementationMetadataBuilder<TImplementation> builder = new(ActorType, stateField => { });
    configure(builder);

    AddImplementation(builder);
    
    return this;
  }

  private IActorMetadataBuilder<TActor> HasFactoryTypeCore(Type factoryType)
  {
    if (!factoryType.IsInterface)
      throw new InvalidOperationException($"'{factoryType}' must be an interface.");

    FactoryType = factoryType;

    return this;
  }

  private IActorMetadataBuilder<TActor> HasInterfaceTypeCore(Type interfaceType)
  {
    if (!interfaceType.IsInterface)
      throw new InvalidOperationException($"'{interfaceType}' must be an interface.");

    _interfaceType = interfaceType;

    return this;
  }
}
