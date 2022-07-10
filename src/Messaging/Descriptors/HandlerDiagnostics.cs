using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors;

/// <summary>
/// Represents an anomaly in the message handler configuration.
/// </summary>
internal class HandlerDiagnostics
{
  public HandlerDiagnostics(Type concreteType, string messageTemplate, params object?[] messageArguments)
  {
    ConcreteType = concreteType;
    MessageTemplate = messageTemplate;
    MessageArguments = messageArguments;
  }

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
}
