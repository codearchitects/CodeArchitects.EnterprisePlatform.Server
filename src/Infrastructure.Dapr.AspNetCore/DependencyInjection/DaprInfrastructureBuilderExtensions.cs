using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.DependencyInjection;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Dapr.State;
using CodeArchitects.Platform.Infrastructure.Messaging;
using CodeArchitects.Platform.Infrastructure.State;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class DaprInfrastructureBuilderExtensions
  {
    /// <summary>
    /// Adds an <see cref="IServiceResolver{TService}"/> of <see cref="IMessageBus"/> and
    /// <see cref="IMessageBus{TMetadata}"/> of <see cref="DaprMetadata"/> to the services.
    /// If configured, also adds a default <see cref="IMessageBus"/> and
    /// <see cref="IMessageBus{TMetadata}"/> of <see cref="DaprMetadata"/>.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns>The same builder.</returns>
    public static IDaprInfrastructureBuilder AddMessageBus(this IDaprInfrastructureBuilder builder)
    {
      builder.Services.AddSingleton<IServiceResolver<IMessageBus>, DaprMessageBusResolver>();
      builder.Services.AddSingleton<IServiceResolver<IMessageBus<DaprMetadata>>, DaprMessageBusResolver>();

      DaprMessagingOptions? options = builder.Configuration?.Service?.Messaging;

      if (options is not null)
      {
        if (!string.IsNullOrWhiteSpace(options.DefaultBus))
        {
          builder.Services.AddSingleton(sp => sp.GetRequiredService<IServiceResolver<IMessageBus>>().Resolve(options.DefaultBus));
          builder.Services.AddSingleton(sp => sp.GetRequiredService<IServiceResolver<IMessageBus<DaprMetadata>>>().Resolve(options.DefaultBus));
        }
      }

      return builder;
    }

    /// <summary>
    /// Enables the executions of all handlers defined in the given assemblies.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="assemblies">The assemblies to scan for handlers. If empty, the calling assembly will be used.</param>
    /// <returns>The same builder.</returns>
    public static IDaprInfrastructureBuilder AddHandlers(this IDaprInfrastructureBuilder builder, params Assembly[] assemblies)
    {
      if (assemblies.Length == 0)
      {
        assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
      }
      HandlerConfiguration configuration = new HandlerConfiguration(assemblies.Distinct());
      builder.Services.AddSingleton<IHandlerConfiguration>(configuration);

      foreach (Type handlerImplementationType in configuration.HandlerMap.Values)
      {
        builder.Services.AddScoped(handlerImplementationType);
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
      builder.Services.AddSingleton<IServiceResolver<IStateStore>, DaprStateStoreResolver>();

      DaprStateOptions? options = builder.Configuration?.Service?.State;

      if (options is not null)
      {
        if (!string.IsNullOrWhiteSpace(options.DefaultStore))
        {
          builder.Services.AddSingleton(sp => sp.GetRequiredService<IServiceResolver<IStateStore>>().Resolve(options.DefaultStore));
        }
      }

      return builder;
    }
  }
}
