namespace CodeArchitects.Platform.Messaging.Descriptors;

/// <summary>
/// Describes the properties of a message handler.
/// </summary>
internal interface IHandlerDescriptor
{
  /// <summary>
  /// The bus name.
  /// </summary>
  string? Bus { get; }

  /// <summary>
  /// The topic name.
  /// </summary>
  string? Topic { get; }

  /// <summary>
  /// The handler interface type.
  /// </summary>
  /// <remarks>
  /// It can either be a constructed <see cref="IMessageHandler{TMessage}"/> type or a <see cref="IMessageHandler{TMessage, TResult}"/> type.
  /// </remarks>
  Type InterfaceType { get; }

  /// <summary>
  /// The handler concrete type.
  /// </summary>
  Type ConcreteType { get; }

  /// <summary>
  /// The type of the message being handled.
  /// </summary>
  Type MessageType { get; }

  /// <summary>
  /// The type of the result the handler produces.
  /// </summary>
  Type ResultType { get; }

  /// <summary>
  /// True if the handler produces a result, false otherwise.
  /// </summary>
  bool HasResult { get; }

  /// <summary>
  /// True if the handler's result type is a union type, false otherwise.
  /// </summary>
  bool HasUnionResult { get; }

  /// <summary>
  /// When the handler's result type is a union type, returns the list of types that make up the result type.
  /// </summary>
  IReadOnlyList<Type> ResultTypes { get; }
  
  /// <summary>
  /// The output bindings.
  /// </summary>
  IReadOnlyCollection<IOutputBindingDescriptor> OutputBindingDescriptors { get; }
}
