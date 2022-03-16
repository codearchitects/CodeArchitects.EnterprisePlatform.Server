using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using Microsoft.Extensions.Configuration;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;

/// <summary>
/// Implementation of <see cref="IDaprConfigurationBuilder"/>.
/// </summary>
internal class DaprConfigurationBuilder : IDaprConfigurationBuilder
{
  private ServiceOptions? _serviceConfig;

  public IDaprConfigurationBuilder AddServiceOptions(IConfigurationSection serviceConfiguration)
  {
    return AddServiceOptionsCore(serviceConfiguration);
  }

  public IDaprConfigurationBuilder AddServiceOptions(IConfiguration configuration)
  {
    return AddServiceOptionsCore(configuration.GetSection("Caep:Dapr"));
  }

  private IDaprConfigurationBuilder AddServiceOptionsCore(IConfigurationSection serviceConfiguration)
  {
    _serviceConfig = new ServiceOptions();
    serviceConfiguration.Bind(_serviceConfig);
    return this;
  }

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
}
