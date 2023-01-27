using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

public interface IActorMetadataBuilder<TActor>
  where TActor : class
{
  IActorMetadataBuilder<TActor> IsVirtual(bool isVirtual = true);

  IActorMetadataBuilder<TActor> HasInterfaceType(Type interfaceType);

  IActorMetadataBuilder<TActor> HasInterfaceType<TInterface>()
    where TInterface : class;

  IActorMetadataBuilder<TActor> HasFactoryType(Type factoryType);

  IActorMetadataBuilder<TActor> HasFactoryType<TFactory>()
    where TFactory : class;

  IActorMetadataBuilder<TActor> HasActorConstructor(Expression<Func<IConstructorArgumentSpec, TActor>> constructorExpression);

  IActorMetadataBuilder<TActor> HasMethod<TReturn>(Expression<Func<TActor, IMethodArgumentSpec, TReturn>> methodExpression, Action<IMethodMetadataBuilder> configure);

  IActorMetadataBuilder<TActor> HasImplementation<TImplementation>(Action<IImplementationMetadataBuilder<TImplementation>> configure)
    where TImplementation : class;
}
