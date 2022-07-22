using CodeArchitects.Platform.Messaging.AspNetCore.Configuration;
using CodeArchitects.Platform.Messaging.Bindings;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

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
  /// Registers an output binding to the application services with the given lifetime.
  /// </summary>
  /// <param name="outputBindingType">The output binding type. It must implement <see cref="IOutputBinding{TMetadata}"/>.</param>
  /// <param name="lifetime">The service lifetime.</param>
  /// <returns>The builder.</returns>
  IDaprMessagingOptionsBuilder RegisterOutputBinding(Type outputBindingType, ServiceLifetime lifetime = ServiceLifetime.Scoped);

  /// <summary>
  /// Registers an alias for an output metadata type that can be used instead of its fully-qualified name in configuration, prefixing it with '$'.
  /// </summary>
  /// <param name="alias">The alias.</param>
  /// <param name="metadataType">The metadata type.</param>
  /// <returns>The builder.</returns>
  IDaprMessagingOptionsBuilder RegisterOutputMetadataAlias(string alias, Type metadataType);
}
