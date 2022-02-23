using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.DependencyInjection;
using System;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IDaprInfrastructureBuilder"/>.
/// </summary>
public static class DaprInfrastructureServiceCollectionExtensions
{
  /// <summary>
  /// Adds basic Dapr infrastructure services to the <see cref="IServiceCollection"/>.
  /// </summary>
  /// <param name="services">The service collection to configure.</param>
  /// <param name="configuration">A Dapr configuration instance.</param>
  /// <returns>An <see cref="IDaprInfrastructureBuilder"/> that can be used to further configure the Dapr infrastructure services.</returns>
  public static IDaprInfrastructureBuilder AddDaprInfrastructure(this IServiceCollection services, DaprConfiguration? configuration = null)
  {
    if (configuration is not null)
    {
      services.AddSingleton(configuration);
    }
    return new DaprInfrastructureBuilder(services, configuration);
  }

  /// <summary>
  /// Adds basic Dapr infrastructure services to the <see cref="IServiceCollection"/>.
  /// </summary>
  /// <param name="services">The service collection to configure.</param>
  /// <param name="configure">A configuration building action.</param>
  /// <returns>An <see cref="IDaprInfrastructureBuilder"/> that can be used to further configure the Dapr infrastructure services.</returns>
  public static IDaprInfrastructureBuilder AddDaprInfrastructure(this IServiceCollection services, Action<IDaprConfigurationBuilder> configure)
  {
    DaprConfigurationBuilder builder = new DaprConfigurationBuilder();
    configure(builder);
    DaprConfiguration configuration = builder.Build();
    services.AddSingleton(configuration);
    return new DaprInfrastructureBuilder(services, configuration);
  }
}
