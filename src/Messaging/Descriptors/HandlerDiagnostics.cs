using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors;

/// <summary>
/// Represents an anomaly in the message handler configuration.
/// </summary>
internal class HandlerDiagnostics
{
  private HandlerDiagnostics(int id, Type concreteType, string messageTemplate, params object?[] messageArguments)
  {
    Id = id;
    ConcreteType = concreteType;
    MessageTemplate = messageTemplate;
    MessageArguments = messageArguments;
  }

  /// <summary>
  /// The id of the diagnostics.
  /// </summary>
  public int Id { get; }

  /// <summary>
  /// The handler concrete type.
  /// </summary>
  public Type ConcreteType { get; }

  /// <summary>
  /// Format string of the diagnostics message in message template format.
  /// </summary>
  public string MessageTemplate { get; }

  /// <summary>
  /// An object array that contains zero or more objects to format.
  /// </summary>
  public object?[] MessageArguments { get; }

  /// <summary>
  /// Raised when an handler concrete type is decorated with more than one <see cref="MessageHandlerAttribute"/>.
  /// </summary>
  /// <param name="concreteType">The handler concrete type.</param>
  /// <returns>The diagnostics.</returns>
  public static HandlerDiagnostics MultipleMessageHandlerAttributeOnClass(Type concreteType)
    => new(1, concreteType, $"Multiple {nameof(MessageHandlerAttribute)} found on type {{0}}", concreteType);

  /// <summary>
  /// Raised when a handler method has null bus name for some subscription.
  /// </summary>
  /// <param name="concreteType">The handler concrete type.</param>
  /// <param name="method">The handler method.</param>
  /// <returns>The diagnostics.</returns>
  public static HandlerDiagnostics NullBusOnHandler(Type concreteType, MethodInfo method)
    => new(2, concreteType, "Handler method {0} on type {1} had null bus name for some subscription", method, concreteType);

  /// <summary>
  /// Raised when a handler method has null topic name for some subscription.
  /// </summary>
  /// <param name="concreteType">The handler concrete type.</param>
  /// <param name="method">The handler method.</param>
  /// <returns>The diagnostics.</returns>
  public static HandlerDiagnostics NullTopicOnHandler(Type concreteType, MethodInfo method)
    => new(3, concreteType, "Handler method {0} on type {1} had null topic name for some subscription", method, concreteType);
}
