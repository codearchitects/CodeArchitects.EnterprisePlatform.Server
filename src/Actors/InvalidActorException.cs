using CodeArchitects.Platform.Actors.Messaging;
using System.Reflection;
using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Exception thrown when an actor is configured invalidly.
/// </summary>
public sealed class InvalidActorException : Exception
{
  private InvalidActorException(Type type, string message)
    : base(message)
  {
    Type = type;
  }

  private InvalidActorException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
    Type = Type.GetType(info.GetString(nameof(Type)));
  }

  /// <summary>
  /// The type that is responsible for the configuration error.
  /// </summary>
  public Type Type { get; }

  /// <inheritdoc/>
  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    base.GetObjectData(info, context);
    info.AddValue(nameof(Type), Type.AssemblyQualifiedName);
  }


  // Class errors

  internal static InvalidActorException GenericActorsAreNotSupported(Type actorType)
    => Create(actorType, ErrorMessages.GenericActorsAreNotSupported, actorType.Name);

  internal static InvalidActorException MissingActorInterface(Type actorType)
    => Create(actorType, ErrorMessages.MissingActorInterface, actorType.Name);

  internal static InvalidActorException AmbiguousActorInterface(Type actorType)
    => Create(actorType, ErrorMessages.AmbiguousActorInterface, actorType.Name);

  internal static InvalidActorException InterfaceNotImplemented(Type actorType)
    => Create(actorType, ErrorMessages.InterfaceNotImplemented, actorType.Name);

  internal static InvalidActorException InvalidInterfaceType(Type actorType, Type interfaceType)
    => Create(actorType, ErrorMessages.InvalidInterfaceType, actorType.Name, interfaceType.Name);

  internal static InvalidActorException PropertiesAreNotSupported(Type actorType, Type interfaceType)
    => Create(actorType, ErrorMessages.PropertiesAreNotSupported, actorType.Name, interfaceType.Name);

  internal static InvalidActorException EventsAreNotSupported(Type actorType, Type interfaceType)
    => Create(actorType, ErrorMessages.EventsAreNotSupported, actorType.Name, interfaceType.Name);

  internal static InvalidActorException ActorCannotBeVirtual(Type actorType)
    => Create(actorType, ErrorMessages.ActorCannotBeVirtual, actorType.Name);

  internal static InvalidActorException MultipleDefaultImplementations(Type actorType)
    => Create(actorType, ErrorMessages.MultipleDefaultImplementations, actorType.Name);

  internal static InvalidActorException ActorNotInherited(Type actorType, Type implementationType)
    => Create(actorType, ErrorMessages.ActorNotInherited, actorType.Name, implementationType.Name);

  internal static InvalidActorException AbstractImplementation(Type actorType, Type implementationType)
    => Create(actorType, ErrorMessages.AbstractImplementation, actorType.Name, implementationType.Name);


  // State/id errors

  internal static InvalidActorException InvalidStateType(Type actorType, Type stateType)
    => Create(actorType, ErrorMessages.InvalidStateType, actorType.Name, stateType.Name);

  internal static InvalidActorException InvalidDefaultValue(Type actorType, MemberInfo member)
    => Create(actorType, ErrorMessages.InvalidDefaultValue, actorType.Name, member.Name);

  internal static InvalidActorException StateMustBeDefinedInBaseActor(Type actorType, Type implementationType)
    => Create(actorType, ErrorMessages.StateMustBeDefinedInBaseActor, actorType.Name, implementationType.Name);

  internal static InvalidActorException MultipleActorIdMembers(Type actorType)
    => Create(actorType, ErrorMessages.MultipleActorIdMembers, actorType.Name);

  internal static InvalidActorException InvalidIdType(Type actorType, Type idType)
    => Create(actorType, ErrorMessages.InvalidIdType, actorType.Name, idType.Name);

  internal static InvalidActorException InvalidIdMember(Type actorType, Type sourceIdType, Type actorIdType)
    => Create(actorType, ErrorMessages.InvalidIdMember, actorType.Name, sourceIdType.Name, actorIdType.Name);


  // Method errors

  internal static InvalidActorException StateComponentNameMismatch(Type implementationType)
    => Create(implementationType, ErrorMessages.StateComponentNameMismatch, implementationType.Name);

  internal static InvalidActorException AmbiguousActorConstructor(Type implementationType)
    => Create(implementationType, ErrorMessages.AmbiguousActorConstructor, implementationType.Name);

  internal static InvalidActorException WrongGenericActorContext(Type implementationType, string? parameterName)
    => Create(implementationType, ErrorMessages.WrongGenericActorContext, implementationType.Name, parameterName);

  internal static InvalidActorException GenericMethodsAreNotSupported(Type actorType, MethodInfo method)
    => Create(actorType, ErrorMessages.GenericMethodsAreNotSupported, actorType.Name, method.Name);

  internal static InvalidActorException InvalidMethodReturnType(Type actorType, MethodInfo method)
    => Create(actorType, ErrorMessages.InvalidMethodReturnType, actorType.Name, method.Name, method.ReturnType.Name);

  internal static InvalidActorException CancellationTokenMustBeLastParameter(Type actorType, MethodInfo method)
    => Create(actorType, ErrorMessages.CancellationTokenMustBeLastParameter, actorType.Name, method.Name);


  // Factory errors

  internal static InvalidActorException GenericFactoriesAreNotSupported(Type actorType, Type factoryType)
    => Create(actorType, ErrorMessages.GenericFactoriesAreNotSupported, actorType.Name, factoryType.Name);

  internal static InvalidActorException MissingActorFactoryType(Type actorType)
    => Create(actorType, ErrorMessages.MissingActorFactoryType, actorType.Name);

  internal static InvalidActorException InvalidActorFactoryType(Type actorType, Type factoryType)
    => Create(actorType, ErrorMessages.InvalidActorFactoryType, actorType.Name, factoryType.Name);

  internal static InvalidActorException AmbiguousActorFactoryType(Type actorType)
    => Create(actorType, ErrorMessages.AmbiguousActorFactoryType, actorType.Name);


  // Other errors

  internal static InvalidActorException DuplicateAttribute(Type type, string attributeName)
    => Create(type, ErrorMessages.DuplicateAttribute, type.Name, attributeName);

  internal static InvalidActorException TargetIsNotAnActor(Type type, Type targetType)
    => Create(type, ErrorMessages.TargetIsNotAnActor, type.Name, targetType.Name);

  internal static InvalidActorException InvalidActorMessage(Type actorType, Type messageType)
    => Create(actorType, ErrorMessages.InvalidActorMessage, actorType.Name, messageType.Name);


  private static InvalidActorException Create(Type actorType, string template, params object?[] args)
  {
    template = "Invalid actor configuration for type '{0}'. " + template;
    return new InvalidActorException(actorType, string.Format(template, args));
  }

  private static class ErrorMessages
  {
    // Class errors
    public const string GenericActorsAreNotSupported = "Actor '{0}' is generic, which is not supported.";
    public const string MissingActorInterface = "An actor must implement an interface which exposes its public methods, but '{0}' does not implement any suitable interface.";
    public const string AmbiguousActorInterface = "Could not infer the actor interface type because '{0}' implements more than one interface.";
    public const string InterfaceNotImplemented = "Type '{0}' does not implement the specified interface type.";
    public const string InvalidInterfaceType = "'{1}' was specified as the interface type of the actor, but it is either not an interface or an invalid one.";
    public const string PropertiesAreNotSupported = "Interface '{1}' defines properties, which are not supported.";
    public const string EventsAreNotSupported = "Interface '{1}' defines events, which are not supported.";
    public const string ActorCannotBeVirtual = "'{0}' cannot be a virtual actor because a default value for its state could not be computed. For simple state types, either configure the its default value or provide a default parameter value. For complex state types, provide a parameterless constructor for the class.";
    public const string MultipleDefaultImplementations = "Multiple default implementations found for actor '{1}'.";
    public const string ActorNotInherited = "Type '{1}' does not inherit from the specified actor type.";
    public const string AbstractImplementation = "Type '{1}' cannot be an actor implementation since it is abstract.";

    // State/id errors
    public const string InvalidStateType = "Type '{1}' cannot be used as an actor state.";
    public const string InvalidDefaultValue = "The provided default value for state component '{1}' is of the wrong type.";
    public const string StateMustBeDefinedInBaseActor = "Each component of the actor's state must be defined in the actor's base class. Implementation '{1}' defines state members.";
    public const string MultipleActorIdMembers = "Multiple actor id members were specified.";
    public const string InvalidIdType = "Type '{1}' cannot be used as id type because it does not define a public static method Parse(string) or Parse(string, IFormatProvider) returning the id type.";
    public const string InvalidIdMember = "An actor member specified an id of type '{1}', but the declared actor id type is '{2}'.";

    // Method errors
    public const string StateComponentNameMismatch = "Could not find a one-to-one correspondence between state members and state constructor parameters.";
    public const string AmbiguousActorConstructor = "Could not infer the actor constructor.";
    public const string WrongGenericActorContext = "The actor context parameter '{1}' must be of type 'IActorContext<{0}>'.";
    public const string GenericMethodsAreNotSupported = "Method '{1}' is generic, which is not supported.";
    public const string InvalidMethodReturnType = "Method '{1}' has an invalid return type '{2}'.";
    public const string CancellationTokenMustBeLastParameter = "Method '{1}' has a CancellationToken parameter which is not its last parameter.";

    // Factory errors
    public const string GenericFactoriesAreNotSupported = "Actor factory '{1}' is generic, which is not supported.";
    public const string MissingActorFactoryType = "Could not find a suitable factory interface for '{0}'.";
    public const string InvalidActorFactoryType = "Actor factory '{1}' is not an interface or has the wrong method signatures.";
    public const string AmbiguousActorFactoryType = "Multiple actor factories were specified for actor '{0}'.";

    // Other errors
    public const string DuplicateAttribute = "Duplicate '{1}' attribute on type '{0}'.";
    public const string TargetIsNotAnActor = "Type '{1}' is not an actor.";
    public const string InvalidActorMessage = $"Message type '{{1}}' does not implement the {nameof(IActorMessage<object>)} interface or it has the wrong actor id type.";
  }
}
