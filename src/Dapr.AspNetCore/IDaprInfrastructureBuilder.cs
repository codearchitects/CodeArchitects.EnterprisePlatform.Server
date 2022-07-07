using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using CodeArchitects.Platform.Dapr.AspNetCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Dapr.AspNetCore;

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
  /// The Dapr configuration section.
  /// </summary>
  IConfigurationSection Configuration { get; }

  /// <summary>
  /// Accessor for Dapr components.
  /// </summary>
  IDaprComponentAccessor ComponentAccessor { get; }

  /// <summary>
  /// Dapr infrastructure services.
  /// </summary>
  IDaprInfrastructureServices DaprServices { get; }

  /// <summary>
  /// The logger.
  /// </summary>
  ILogger Logger { get; set; }
}
