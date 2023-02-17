using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Builder;

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

  IActorMetadataBuilder<TActor> HasState<TState>(Expression<Func<TActor, TState>> memberExpression);

  IActorMetadataBuilder<TActor> HasState<TState>(Expression<Func<TActor, TState>> memberExpression, Action<IStateComponentBuilder<TActor, TState>> configure);

  IActorMetadataBuilder<TActor> HasState(string memberName);

  IActorMetadataBuilder<TActor> HasState<TState>(string memberName, Action<IStateComponentBuilder<TActor, TState>> configure);

  IActorMetadataBuilder<TActor> HasConstructor(ConstructorInfo constructor);

  IActorMetadataBuilder<TActor> HasConstructor(params Type[] parameterTypes);

  IActorMetadataBuilder<TActor> HasConstructor(Expression<Func<IConstructorArgument<TActor>, TActor>> constructorExpression);

  IActorMetadataBuilder<TActor> HasImplementation<TImplementation>()
    where TImplementation : TActor;

  IActorMetadataBuilder<TActor> HasImplementation<TImplementation>(Action<IImplementationMetadataBuilder<TActor, TImplementation>> configure)
    where TImplementation : TActor;
}
