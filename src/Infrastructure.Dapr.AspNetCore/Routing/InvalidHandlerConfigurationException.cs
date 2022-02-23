using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Messaging;
using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Routing;

/// <summary>
/// Exception that is thrown when the <see cref="IMessageHandlerConfiguration"/> is invalid.
/// </summary>
public class InvalidHandlerConfigurationException : Exception
{
  /// <summary>
  /// Creates an instance of <see cref="InvalidHandlerConfigurationException"/>.
  /// </summary>
  /// <param name="handlerType">The <see cref="IMessageHandler{TMessage}"/> type.</param>
  /// <param name="handlerImplementationType">The invalid handler implementation type.</param>
  public InvalidHandlerConfigurationException(Type handlerType, Type handlerImplementationType)
    : base($"The handler type {handlerImplementationType} do not implement the {handlerType.Name} interface.")
  {
    HandlerType = handlerType;
    HandlerImplementationType = handlerImplementationType;
  }

  /// <summary>
  /// The <see cref="IMessageHandler{TMessage}"/> type.
  /// </summary>
  public Type HandlerType { get; }

  /// <summary>
  /// The invalid handler implementation type.
  /// </summary>
  public Type HandlerImplementationType { get; }
}
