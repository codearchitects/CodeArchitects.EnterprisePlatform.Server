using CodeArchitects.Platform.Common.Ioc;
using CodeArchitects.Platform.Dapr.AspNetCore;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;
using CodeArchitects.Platform.Infrastructure.Dapr.State;
using CodeArchitects.Platform.Infrastructure.State;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IDaprInfrastructureBuilder"/>.
/// </summary>
public static class InfrastructureDaprInfrastructureBuilderExtensions
{
  private const string s_stateKey = "State";

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

    DaprStateConfig config = new();
    builder.Configuration.Bind(s_stateKey, config);
    builder.DaprServices.AddService(config);

    StoreInfo info = StoreInfo.Create(builder.ComponentAccessor, config.DefaultStore);
    string? defaultStore = info.GetDefaultStore();

    builder.Services.AddSingleton<IServiceResolver<IStateStore>>(delegate (IServiceProvider services)
    {
      DaprClient dapr = services.GetRequiredService<DaprClient>();
      ILogger logger = services.GetService<ILoggerFactory>()?.CreateLogger("CAEP-State") ?? NullLogger.Instance;
      return new StateStoreResolver(dapr, info, logger);
    });

    if (!string.IsNullOrWhiteSpace(defaultStore))
    {
      builder.Services.AddSingleton(sp => sp.GetRequiredService<IServiceResolver<IStateStore>>().Resolve(defaultStore));
    }

    return builder;
  }
}
