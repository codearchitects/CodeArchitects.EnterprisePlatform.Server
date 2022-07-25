using Castle.Components.DictionaryAdapter;
using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Dapr.AspNetCore;
using CodeArchitects.Platform.Messaging;
using CodeArchitects.Platform.Messaging.AspNetCore;
using CodeArchitects.Platform.Messaging.AspNetCore.Bindings;
using CodeArchitects.Platform.Messaging.AspNetCore.Configuration;
using CodeArchitects.Platform.Messaging.AspNetCore.Descriptors;
using CodeArchitects.Platform.Messaging.AspNetCore.Handlers;
using CodeArchitects.Platform.Messaging.AspNetCore.Utils;
using CodeArchitects.Platform.Messaging.Bindings;
using CodeArchitects.Platform.Messaging.Dapr;
using CodeArchitects.Platform.Messaging.Dapr.AspNetCore;
using CodeArchitects.Platform.Messaging.Dapr.Bindings;
using CodeArchitects.Platform.Messaging.Descriptors;
using CodeArchitects.Platform.Messaging.Descriptors.Implementation;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IDaprInfrastructureBuilder"/>.
/// </summary>
public static class MessagingDaprInfrastructureBuilderExtensions
{
  private const string s_messagingKey = "Messaging";

  /// <summary>
  /// Adds messaging capabilities to the application via the Dapr pubsub API.
  /// </summary>
  /// <remarks>
  /// This method adds an <see cref="IServiceResolver{TService}"/> of <see cref="IMessageBus"/> to the services and,
  /// if a default bus is configured, an <see cref="IMessageBus"/> also.
  /// </remarks>
  /// <param name="builder">The Dapr infrastructure builder.</param>
  /// <returns></returns>
  public static IDaprInfrastructureBuilder AddMessaging(this IDaprInfrastructureBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return AddMessagingCore(builder, optionsBuilder => { });
  }

  /// <summary>
  /// Adds messaging capabilities to the application via the Dapr pubsub API.
  /// </summary>
  /// <remarks>
  /// This method adds an <see cref="IServiceResolver{TService}"/> of <see cref="IMessageBus"/> to the services and,
  /// if a default bus is configured, an <see cref="IMessageBus"/> also. The configuration action can be used to
  /// configure messaging options and to register message handlers.
  /// </remarks>
  /// <param name="builder">The Dapr infrastructure builder.</param>
  /// <param name="configure">An action that can be used to configure the messaging options.</param>
  /// <returns>The builder.</returns>
  public static IDaprInfrastructureBuilder AddMessaging(this IDaprInfrastructureBuilder builder, Action<IDaprMessagingOptionsBuilder> configure)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    return AddMessagingCore(builder, configure);
  }

  private static IDaprInfrastructureBuilder AddMessagingCore(IDaprInfrastructureBuilder builder, Action<IDaprMessagingOptionsBuilder> configure)
  {
    MessagingConfig config = new();
    builder.Configuration.Bind(s_messagingKey, config);
    builder.DaprServices.AddService(config);

    DaprMessagingOptionsBuilder optionsBuilder = new(config);
    RegisterDefaultAliases(optionsBuilder);
    configure(optionsBuilder);

    MessageBiMap messageMap = new();
    MessagingInfo info = MessagingInfo.Create(messageMap, builder.ComponentAccessor, config.DefaultBus);
    string? defaultBus = info.GetDefaultBus();

    IMessagingDescriptor descriptor = CreateDescriptor(builder, optionsBuilder, config, defaultBus);
    builder.DaprServices.AddService(descriptor);

    foreach (IMessageDescriptor messageDescriptor in descriptor.MessageDescriptors)
    {
      messageMap.Add(messageDescriptor.Type, messageDescriptor.Name);
    }

    AddHandlers(builder, descriptor);

    AddMessageBus(builder, info, defaultBus);

    AddTopicRouterFactory(builder, messageMap);

    AddDefaultOutputBindings(builder.Services, info);

    AddCustomOutputBindings(builder.Services, optionsBuilder);

    builder.Services.AddSingleton<MessagingMarkerService>();

    return builder;
  }

  private static void AddDefaultOutputBindings(IServiceCollection services, IMessagingInfo info)
  {
    services.AddSingleton<IOutputBinding<IMessageBusOutputMetadata>>(delegate (IServiceProvider services)
    {
      DaprClient dapr = services.GetRequiredService<DaprClient>();
      ILogger logger = services.CreateMessagingLogger();
      return new MessageBusOutputBinding(dapr, info, logger);
    });

    services.AddSingleton<IOutputBinding<IStateStoreOutputMetadata>, StateStoreOutputBinding>();
  }

  private static void AddCustomOutputBindings(IServiceCollection services, DaprMessagingOptionsBuilder optionsBuilder)
  {
    foreach (ServiceDescriptor serviceDescriptor in optionsBuilder.BindingServiceDescriptors)
    {
      services.Add(serviceDescriptor);
    }
  }

  private static IMessagingDescriptor CreateDescriptor(IDaprInfrastructureBuilder builder, DaprMessagingOptionsBuilder optionsBuilder, MessagingConfig config, string? defaultBus)
  {
    List<HandlerDiagnostics> diagnosticCollection = new();
    ConfigurationDescriptorFactory configurationDescriptorFactory = new(new DictionaryAdapterFactory(), builder.Logger, optionsBuilder.TypeAliases);
    IMessagingDescriptor descriptorFromReflection = MessagingDescriptor.Create(optionsBuilder.HandlerTypes, optionsBuilder.MessageTypes, defaultBus, MessageBus.DefaultTopic, diagnosticCollection);
    IMessagingDescriptor descriptor = MessagingDescriptor.Merge(
      first: descriptorFromReflection,
      second: configurationDescriptorFactory.Create(config.Handlers, defaultBus, MessageBus.DefaultTopic),
      diagnosticCollection: diagnosticCollection);

    foreach (HandlerDiagnostics diagnostics in diagnosticCollection)
    {
      builder.Logger.LogWarning(diagnostics.MessageTemplate, diagnostics.MessageArguments);
    }

    return descriptor;
  }

  private static void AddHandlers(IDaprInfrastructureBuilder builder, IMessagingDescriptor descriptor)
  {
    IEnumerable<Type> handlerConcreteTypes = descriptor.HandlerDescriptors
      .Select(descr => descr.ConcreteType)
      .Distinct();
    foreach (Type handlerConcreteType in handlerConcreteTypes)
    {
      builder.Services.AddScoped(handlerConcreteType);
    }
  }

  private static void AddMessageBus(IDaprInfrastructureBuilder builder, IMessagingInfo info, string? defaultBus)
  {
    builder.Services.AddSingleton<IServiceResolver<IMessageBus>>(delegate (IServiceProvider services)
    {
      DaprClient dapr = services.GetRequiredService<DaprClient>();
      ILogger logger = services.CreateMessagingLogger();
      return new MessageBusResolver(dapr, info, logger);
    });

    if (!string.IsNullOrWhiteSpace(defaultBus))
    {
      builder.Services.AddSingleton(sp => sp.GetRequiredService<IServiceResolver<IMessageBus>>().Resolve(defaultBus));
    }
  }

  private static void AddTopicRouterFactory(IDaprInfrastructureBuilder builder, IMessageBiMap messageMap)
  {
    builder.Services.AddSingleton<ITopicRouterFactory>(delegate (IServiceProvider services)
    {
      OutputActionFactory outputActionFactory = new OutputActionFactory();
      HandlerDelegateFactory delegateFactory = new HandlerDelegateFactory(services, outputActionFactory);
      return new TopicRouterFactory(delegateFactory, messageMap);
    });
  }

  private static void RegisterDefaultAliases(IDaprMessagingOptionsBuilder optionsBuilder)
  {
    optionsBuilder.RegisterOutputMetadataAlias("MessageBus", typeof(IMessageBusOutputMetadata));
    optionsBuilder.RegisterOutputMetadataAlias("StateStore", typeof(IStateStoreOutputMetadata));
  }

  #region Deprecated methods

  /// <summary>
  /// Adds an <see cref="IServiceResolver{TService}"/> of <see cref="IMessageBus"/> to the services.
  /// If configured, also adds a default <see cref="IMessageBus"/>.
  /// </summary>
  /// <param name="builder">The builder instance.</param>
  /// <returns>The same builder.</returns>
  [Obsolete($"{nameof(AddMessageBus)} is deprecated and will be removed in the next release. Use {nameof(AddMessaging)} instead.")]
  public static IDaprInfrastructureBuilder AddMessageBus(this IDaprInfrastructureBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    if (builder.Services.Any(descr => descr.ServiceType == typeof(MessagingMarkerService)))
      return builder;

    return builder.AddMessaging();
  }

  /// <summary>
  /// Enables the executions of all handlers defined in the given assemblies.
  /// </summary>
  /// <param name="builder">The builder instance.</param>
  /// <param name="assemblies">The assemblies to scan for handlers. If empty, the calling assembly will be used.</param>
  /// <returns>The same builder.</returns>
  [Obsolete($"{nameof(AddMessageHandlers)} is deprecated and will be removed in the next release. Use {nameof(AddMessaging)} instead.")]
  public static IDaprInfrastructureBuilder AddMessageHandlers(this IDaprInfrastructureBuilder builder, params Assembly[] assemblies)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));
    if (assemblies is null)
      throw new ArgumentNullException(nameof(assemblies));

    if (builder.Services.Any(descr => descr.ServiceType == typeof(MessagingMarkerService)))
      return builder;

    if (assemblies.Length == 0)
    {
      assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
    }

    return builder.AddMessaging(optionsBuilder =>
    {
      foreach (Assembly assembly in assemblies)
      {
        optionsBuilder.ScanAssembly(assembly);
      }
    });
  }

  #endregion
}
