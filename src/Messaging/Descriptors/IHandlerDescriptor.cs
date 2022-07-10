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
  /// The output bindings.
  /// </summary>
  IReadOnlyCollection<IOutputBindingDescriptor> OutputBindingDescriptors { get; }
}
