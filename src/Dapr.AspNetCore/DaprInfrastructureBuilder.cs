using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using CodeArchitects.Platform.Dapr.AspNetCore.Services;
using CodeArchitects.Platform.Dapr.AspNetCore.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CodeArchitects.Platform.Dapr.AspNetCore;

internal class DaprInfrastructureBuilder : IDaprInfrastructureBuilder
{
  private readonly LoggerReference _logger;

  public DaprInfrastructureBuilder(
    LoggerReference logger,
    IServiceCollection services,
    IConfigurationSection configuration,
    DaprComponentAccessor componentAccessor,
    DaprInfrastructureServices daprServices)
  {
    _logger = logger;
    Services = services;
    Configuration = configuration;
    ComponentAccessor = componentAccessor;
    DaprServices = daprServices;
  }

  public IServiceCollection Services { get; }

  public IConfigurationSection Configuration { get; }

  public DaprComponentAccessor ComponentAccessor { get; }

  public DaprInfrastructureServices DaprServices { get; }

  public ILogger Logger
  {
    get => _logger;
    set
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      _logger.Logger = value;
    }
  }

  IDaprComponentAccessor IDaprInfrastructureBuilder.ComponentAccessor => ComponentAccessor;

  IDaprInfrastructureServices IDaprInfrastructureBuilder.DaprServices => DaprServices;

  public static DaprInfrastructureBuilder Create(IServiceCollection services, IConfigurationSection configuration)
  {
    LoggerReference logger = new(new NullLogger<DaprInfrastructureBuilder>());
    DaprComponentAccessor componentAccessor = DaprComponentAccessor.Create(new ComponentReader(), logger);
    DaprInfrastructureServices daprServices = DaprInfrastructureServices.Create();

    return new DaprInfrastructureBuilder(logger, services, configuration, componentAccessor, daprServices);
  }
}
