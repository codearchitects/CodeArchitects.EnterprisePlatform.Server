namespace CodeArchitects.Platform.Messaging.Descriptors;

/// <summary>
/// Describes the properties of a message handler.
/// </summary>
internal interface IHandlerDescriptor
{
  /// <summary>
  /// The handler concrete type.
  /// </summary>
  Type ConcreteType { get; }

  /// <summary>
  /// The identities of the handler.
  /// </summary>
  IReadOnlyCollection<IHandlerIdentityDescriptor> IdentityDescriptors { get; }
}
