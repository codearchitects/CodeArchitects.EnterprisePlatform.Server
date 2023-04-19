using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Builder;

/// <summary>
/// A builder that can be used to configure an actor.
/// </summary>
/// <typeparam name="TActor">The actor type.</typeparam>
public interface IActorMetadataBuilder<TActor>
  where TActor : class
{
  /// <summary>
  /// Specifies whether an actor is virtual, meaning it does not need to be explicitly created with an initial state value.
  /// </summary>
  /// <param name="isVirtual"><see langword="true"/> if the actor is virtual, otherwise <see langword="false"/>.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> IsVirtual(bool isVirtual = true);

  /// <summary>
  /// Specifies the interface type to associate to the actor.
  /// </summary>
  /// <remarks>
  /// A call to this method is mandatory if the actors implements multiple interfaces, except for known ones.
  /// </remarks>
  /// <param name="interfaceType">The interface type used to invoke the actor.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasInterfaceType(Type interfaceType);

  /// <summary>
  /// Specifies the interface type to associate to the actor.
  /// </summary>
  /// <remarks>
  /// A call to this method is mandatory if the actors implements multiple interfaces, except for known ones.
  /// </remarks>
  /// <typeparam name="TInterface">The interface type used to invoke the actor.</typeparam>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasInterfaceType<TInterface>()
    where TInterface : class;

  /// <summary>
  /// Specifies the factory interface that is used to create proxies of the actor.
  /// </summary>
  /// <remarks>
  /// A call to this method is mandatory.
  /// </remarks>
  /// <param name="factoryType">The factory type.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasFactoryType(Type factoryType);

  /// <summary>
  /// Specifies the factory interface that is used to create proxies of the actor.
  /// </summary>
  /// <remarks>
  /// A call to this method is mandatory.
  /// </remarks>
  /// <typeparam name="TFactory">The factory type.</typeparam>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasFactoryType<TFactory>()
    where TFactory : class;

  /// <summary>
  /// Indicates that an actor's member is a state component.
  /// </summary>
  /// <typeparam name="TState">The type of the member.</typeparam>
  /// <param name="memberExpression">An expression that represents a field or property access.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasState<TState>(Expression<Func<TActor, TState>> memberExpression);

  /// <summary>
  /// Indicates that an actor's member is a state component.
  /// </summary>
  /// <typeparam name="TState">The type of the member.</typeparam>
  /// <param name="memberExpression">An expression that represents a field or property access.</param>
  /// <param name="configure">An action used to configure the state component.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasState<TState>(Expression<Func<TActor, TState>> memberExpression, Action<IStateComponentBuilder<TActor, TState>> configure);

  /// <summary>
  /// Indicates that an actor's member is a state component.
  /// </summary>
  /// <param name="memberName">The name of the member.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasState(string memberName);

  /// <summary>
  /// Indicates that an actor's member is a state component.
  /// </summary>
  /// <typeparam name="TState">The type of the member.</typeparam>
  /// <param name="memberName">The name of the member.</param>
  /// <param name="configure">An action used to configure the state component.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasState<TState>(string memberName, Action<IStateComponentBuilder<TActor, TState>> configure);

  /// <summary>
  /// Indicates that an actor's member represents the actor's id.
  /// </summary>
  /// <typeparam name="TActorId">The type of the actor id.</typeparam>
  /// <param name="memberExpression">An expression that represents a field or property access.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasId<TActorId>(Expression<Func<TActor, TActorId>> memberExpression);

  /// <summary>
  /// Indicates that an actor's member represents the actor's id.
  /// </summary>
  /// <param name="memberName">The name of the member.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasId(string memberName);

  /// <summary>
  /// Specifies which constructor to use for instantiating the actor.
  /// </summary>
  /// <param name="constructor">The actor's constructor.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasConstructor(ConstructorInfo constructor);

  /// <summary>
  /// Specifies the parameters of the constructor to use for instantiating the actor.
  /// </summary>
  /// <param name="parameterTypes">The actor's constructor parameters.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasConstructor(params Type[] parameterTypes);

  /// <summary>
  /// Specifies which constructor to use for instantiating the actor.
  /// </summary>
  /// <param name="constructorExpression">An expression that represents the creation of the actor.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasConstructor(Expression<Func<IConstructorArgument<TActor>, TActor>> constructorExpression);

  /// <summary>
  /// Registers an implementation of the actor.
  /// </summary>
  /// <typeparam name="TImplementation">The implementation type.</typeparam>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasImplementation<TImplementation>()
    where TImplementation : TActor;

  /// <summary>
  /// Registers an implementation of the actor.
  /// </summary>
  /// <typeparam name="TImplementation">The implementation type.</typeparam>
  /// <param name="configure">An action used to configure the implementation.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasImplementation<TImplementation>(Action<IImplementationMetadataBuilder<TActor, TImplementation>> configure)
    where TImplementation : TActor;

  /// <summary>
  /// Specifies what type to use for the actor id.
  /// </summary>
  /// <param name="actorIdType">The id type.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasIdType(Type actorIdType);

  /// <summary>
  /// Specifies what type to use for the actor id.
  /// </summary>
  /// <typeparam name="TActorId">The id type.</typeparam>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> HasIdType<TActorId>();

  /// <summary>
  /// Indicates that a method is a message handler.
  /// </summary>
  /// <param name="messageType">The type of the message.</param>
  /// <param name="configure">An action used to configure the handler method.</param>
  /// <returns>The same builder.</returns>
  IActorMetadataBuilder<TActor> IsMessageHandler(Type messageType, Action<IMessageHandlerMetadataBuilder> configure);
}
