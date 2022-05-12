using CodeArchitects.Platform.Common.Internals;
using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using CodeArchitects.Platform.Dapr.AspNetCore.DependencyInjection;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Dapr.State;
using CodeArchitects.Platform.Infrastructure.Messaging;
using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IDaprInfrastructureBuilder"/>.
/// </summary>
public static class InfrastructureDaprInfrastructureBuilderExtensions
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

    DaprMessagingOptions options = builder.DaprConfigurationBuilder.GetOrAdd<DaprMessagingOptions>(s_messagingKey, AddMessageBusNames);
    string? defaultBus = options.GetDefaultBus();

    builder.Services.AddSingleton<IServiceResolver<IMessageBus>>(delegate (IServiceProvider services)
    {
      return new MessageBusResolver(services.GetRequiredService<DaprClient>(), options, services.GetService<ILogger<MessageBusResolver>>());
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

    DaprMessagingOptions options = builder.DaprConfigurationBuilder.GetOrAdd<DaprMessagingOptions>(s_messagingKey, AddMessageBusNames);

    MessagingConfiguration configuration = MessagingConfiguration.Create(assemblies.Distinct(), options);
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

  /// <summary>
  /// Adds an <see cref="IServiceResolver{TService}"/> of <see cref="IStateStore"/> to the services.
  /// If configured, also adds a default <see cref="IStateStore"/>.
  /// </summary>
  /// <param name="builder">The builder instance.</param>
  /// <returns>The same builder.</returns>
  public static IDaprInfrastructureBuilder AddStateStore(this IDaprInfrastructureBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    DaprStateOptions options = builder.DaprConfigurationBuilder.GetOrAdd<DaprStateOptions>(s_stateKey, AddStateStoreNames);
    string? defaultStore = options.GetDefaultStore();

    builder.Services.AddSingleton<IServiceResolver<IStateStore>>(delegate (IServiceProvider services)
    {
      return new StateStoreResolver(services.GetRequiredService<DaprClient>(), options, services.GetService<ILogger<StateStoreResolver>>());
    });

    if (!string.IsNullOrWhiteSpace(defaultStore))
    {
      builder.Services.AddSingleton(sp => sp.GetRequiredService<IServiceResolver<IStateStore>>().Resolve(defaultStore));
    }

    return builder;
  }

  private static IEnumerable<string> GetComponentNames(IEnumerable<ComponentSchema> components, string componentType)
  {
    return components
      .Where(c => c.Spec.Type.StartsWith(componentType))
      .Select(c => c.Metadata.Name);
  }

  private static void AddMessageBusNames(DaprMessagingOptions options, IDaprConfiguration configuration)
  {
    foreach (string busName in GetComponentNames(configuration.Components, "pubsub"))
    {
      options.BusNames.Add(busName);
    }
  }

  private static void AddStateStoreNames(DaprStateOptions options, IDaprConfiguration configuration)
  {
    foreach (string storeName in GetComponentNames(configuration.Components, "state"))
    {
      options.StoreNames.Add(storeName);
    }
  }
}
