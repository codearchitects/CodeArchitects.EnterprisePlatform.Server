using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using Microsoft.Extensions.Configuration;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;

/// <summary>
/// Implementation of <see cref="IDaprConfigurationBuilder"/>.
/// </summary>
internal class DaprConfigurationBuilder : IDaprConfigurationBuilder
{
  private ServiceOptions? _serviceConfig;

  /// <inheritdoc cref="IDaprConfigurationBuilder.AddServiceOptions(IConfigurationSection)"/>
  public DaprConfigurationBuilder AddServiceOptions(IConfigurationSection serviceConfiguration)
  {
    _serviceConfig = new ServiceOptions();
    serviceConfiguration.Bind(_serviceConfig);
    return this;
  }

  /// <inheritdoc cref="IDaprConfigurationBuilder.AddServiceOptions(IConfiguration)"/>
  public DaprConfigurationBuilder AddServiceOptions(IConfiguration configuration)
    => AddServiceOptions(configuration.GetSection("Caep:Dapr"));

  /// <summary>
  /// Builds a <see cref="DaprConfiguration"/> instance.
  /// </summary>
  /// <returns>The built instance.</returns>
  public DaprConfiguration Build()
  {
    return new DaprConfiguration
    {
      Service = _serviceConfig
    };
  }

  IDaprConfigurationBuilder IDaprConfigurationBuilder.AddServiceOptions(IConfigurationSection serviceConfiguration)
    => AddServiceOptions(serviceConfiguration);

  IDaprConfigurationBuilder IDaprConfigurationBuilder.AddServiceOptions(IConfiguration configuration)
    => AddServiceOptions(configuration);
}
