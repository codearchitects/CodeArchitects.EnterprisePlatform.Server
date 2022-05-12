using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CodeArchitects.Platform.Dapr.AspNetCore.DependencyInjection;

internal class DaprInfrastructureBuilder : IDaprInfrastructureBuilder, ILoggerAccessor
{
  private readonly DaprConfiguration _daprConfiguration;
  private ILogger _logger;

  public DaprInfrastructureBuilder(IServiceCollection services, IConfiguration configuration, Func<ILoggerAccessor, DaprConfiguration> daprConfigurationFactory)
  {
    Services = services;
    Configuration = configuration;
    _daprConfiguration = daprConfigurationFactory(this);
    _logger = new NullLogger<IDaprInfrastructureBuilder>();
  }

  public IServiceCollection Services { get; }

  public IConfiguration Configuration { get; }

  public IDaprConfigurationBuilder DaprConfigurationBuilder => _daprConfiguration;

  public ILogger Logger
  {
    get => _logger;
    set
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      _logger = value;
    }
  }
}
