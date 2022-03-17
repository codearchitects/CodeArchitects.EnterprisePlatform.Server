using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.DependencyInjection;

/// <summary>
/// An <see cref="IDaprInfrastructureBuilder"/> implementation.
/// </summary>
internal class DaprInfrastructureBuilder : IDaprInfrastructureBuilder
{
  /// <summary>
  /// Creates a new <see cref="DaprInfrastructureBuilder"/> instance.
  /// </summary>
  /// <param name="services">The service collection to configure.</param>
  /// <param name="configuration">The Dapr configuration object.</param>
  public DaprInfrastructureBuilder(IServiceCollection services, DaprConfiguration configuration)
  {
    Services = services;
    Configuration = configuration;
  }

  public IServiceCollection Services { get; }

  public DaprConfiguration Configuration { get; }
}
