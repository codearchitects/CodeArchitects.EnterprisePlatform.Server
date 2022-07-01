using CodeArchitects.Platform.Common.Internals;
using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using CodeArchitects.Platform.Dapr.AspNetCore.DependencyInjection;
using CodeArchitects.Platform.Messaging;
using CodeArchitects.Platform.Messaging.AspNetCore.Utils;
using CodeArchitects.Platform.Messaging.Dapr;
using CodeArchitects.Platform.Messaging.Dapr.AspNetCore;
using CodeArchitects.Platform.Messaging.Dapr.AspNetCore.Configuration;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IDaprInfrastructureBuilder"/>.
/// </summary>
public static class MessagingDaprInfrastructureBuilderExtensions
{
  private const string s_messagingKey = "Messaging";
  private const string s_stateKey = "State";

  /// <summary>
  /// Adds an <see cref="IServiceResolver{TService}"/> of <see cref="IMessageBus"/> to the services.
  /// If configured, also adds a default <see cref="IMessageBus"/>.
  /// </summary>
  /// <param name="builder">The builder instance.</param>
  /// <returns>The same builder.</returns>
  public static IDaprInfrastructureBuilder AddMessageBus(this IDaprInfrastructureBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    MessagingConfig config = builder.DaprConfigurationBuilder.GetOrAdd<MessagingConfig>(s_messagingKey, AddMessageBusNames);
    string? defaultBus = config.GetDefaultBus();

    MessagingInfo info = new MessagingInfo(null, config);

    builder.Services.AddSingleton<IServiceResolver<IMessageBus>>(delegate (IServiceProvider services)
    {
      return new MessageBusResolver(services.GetRequiredService<DaprClient>(), services.TryGetLogger(), info);
    });

    if (!string.IsNullOrWhiteSpace(defaultBus))
    {
      builder.Services.AddSingleton(sp => sp.GetRequiredService<IServiceResolver<IMessageBus>>().Resolve(defaultBus));
    }

    return builder;
  }

  /// <summary>
  /// Enables the executions of all handlers defined in the given assemblies.
  /// </summary>
  /// <param name="builder">The builder instance.</param>
  /// <param name="assemblies">The assemblies to scan for handlers. If empty, the calling assembly will be used.</param>
  /// <returns>The same builder.</returns>
  public static IDaprInfrastructureBuilder AddMessageHandlers(this IDaprInfrastructureBuilder builder, params Assembly[] assemblies)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));
    if (assemblies is null)
      throw new ArgumentNullException(nameof(assemblies));

    if (assemblies.Length == 0)
    {
      assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
    }

    MessagingConfig config = builder.DaprConfigurationBuilder.GetOrAdd<MessagingConfig>(s_messagingKey, AddMessageBusNames);

    MessagingConfiguration configuration = MessagingConfiguration.Create(assemblies.Distinct(), config);
    TopicDelegateFactory factory = TopicDelegateFactory.Create(configuration);

    builder.Services.AddSingleton<MessageHandlerMarkerService>();
    builder.Services.AddSingleton<IMessagingConfiguration>(configuration);
    builder.Services.AddSingleton<ITopicDelegateFactory>(factory);

    foreach (ImplementationPair pair in configuration.HandlerMap.Values)
    {
      builder.Services.AddScoped(pair.ImplementationType);
    }

    return builder;
  }

  private static IEnumerable<string> GetComponentNames(IEnumerable<ComponentSchema> components, string componentType)
  {
    return components
      .Where(c => c.Spec.Type.StartsWith(componentType))
      .Select(c => c.Metadata.Name);
  }

  private static void AddMessageBusNames(MessagingConfig options, IDaprConfiguration configuration)
  {
    if (configuration.Components is not { } components)
      return;

    options.BusNames ??= new();
    foreach (string busName in GetComponentNames(components, "pubsub"))
    {
      options.BusNames.Add(busName);
    }
  }
}
