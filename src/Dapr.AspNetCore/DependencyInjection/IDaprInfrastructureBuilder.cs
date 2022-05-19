using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Dapr.AspNetCore.DependencyInjection;

/// <summary>
/// An interface for configuring Dapr infrastructure services.
/// </summary>
public interface IDaprInfrastructureBuilder
{
  /// <summary>
  /// The service collection where the infrastructure services are configured.
  /// </summary>
  IServiceCollection Services { get; }

  /// <summary>
  /// The configuration instance.
  /// </summary>
  IConfiguration Configuration { get; }

  /// <summary>
  /// The Dapr configuration builder.
  /// </summary>
  IDaprConfigurationBuilder DaprConfigurationBuilder { get; }

  /// <summary>
  /// The logger.
  /// </summary>
  ILogger Logger { get; set; }
}
