using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using CodeArchitects.Platform.Dapr.AspNetCore.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.DependencyInjection;

public class FakeDaprInfrastructureBuilder : IDaprInfrastructureBuilder
{
  public FakeDaprInfrastructureBuilder(IServiceCollection services, IConfiguration configuration, IDaprConfigurationBuilder daprConfigurationBuilder, ILogger logger)
  {
    Services = services;
    Configuration = configuration;
    DaprConfigurationBuilder = daprConfigurationBuilder;
    Logger = logger;
  }

  public IServiceCollection Services { get; }

  public IConfiguration Configuration { get; }

  public IDaprConfigurationBuilder DaprConfigurationBuilder { get; }

  public ILogger Logger { get; set; }
}
