namespace CodeArchitects.Platform.Messaging.Descriptors;

/// <summary>
/// Describes the properties of an handler identity.
/// </summary>
internal interface IHandlerIdentityDescriptor
{
  /// <summary>
  /// The handler interface type.
  /// </summary>
  /// <remarks>
  /// It can either be a constructed <see cref="IMessageHandler{TMessage}"/> type or a <see cref="IMessageHandler{TMessage, TResult}"/> type.
  /// </remarks>
  Type InterfaceType { get; }

  /// <summary>
  /// The type of the message being handled.
  /// </summary>
  Type MessageType { get; }

  /// <summary>
  /// The type of the result the handler produces.
  /// </summary>
  Type ResultType { get; }
  string? Bus { get; }
  string? Topic { get; }
  IReadOnlyCollection<IOutputBindingDescriptor> OutputBindingDescriptors { get; }
}
