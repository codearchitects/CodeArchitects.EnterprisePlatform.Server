using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.DependencyInjection;

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
  /// The Dapr configuration.
  /// </summary>
  DaprConfiguration? Configuration { get; }
}
