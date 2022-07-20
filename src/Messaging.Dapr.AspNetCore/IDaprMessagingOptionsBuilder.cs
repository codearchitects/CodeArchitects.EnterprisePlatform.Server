using CodeArchitects.Platform.Messaging.AspNetCore.Configuration;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

/// <summary>
/// A fluent builder that can be used to configure messaging options.
/// </summary>
public interface IDaprMessagingOptionsBuilder
{
  /// <summary>
  /// Further configures the messaging configuration.
  /// </summary>
  /// <param name="configure">The configuration action.</param>
  /// <returns>The builder.</returns>
  IDaprMessagingOptionsBuilder Configure(Action<MessagingConfig> configure);

  /// <summary>
  /// Registers an handler using its attributes.
  /// </summary>
  /// <param name="handlerType">The handler type.</param>
  /// <returns>The builder.</returns>
  IDaprMessagingOptionsBuilder AddHandler(Type handlerType);

  /// <summary>
  /// Registers a message type.
  /// </summary>
  /// <param name="messageType">The message type.</param>
  /// <returns>The builder.</returns>
  IDaprMessagingOptionsBuilder AddMessage(Type messageType);

  /// <summary>
  /// Scans the assembly for registering messages and message handlers.
  /// </summary>
  /// <param name="assembly">The assembly to scan.</param>
  /// <returns>The builder.</returns>
  IDaprMessagingOptionsBuilder ScanAssembly(Assembly assembly);

  /// <summary>
  /// Scans the assembly of the given type for registering messages and message handlers.
  /// </summary>
  /// <typeparam name="T">A marker type of the assembly.</typeparam>
  /// <returns>The builder.</returns>
  IDaprMessagingOptionsBuilder ScanAssemblyOfType<T>();
}
