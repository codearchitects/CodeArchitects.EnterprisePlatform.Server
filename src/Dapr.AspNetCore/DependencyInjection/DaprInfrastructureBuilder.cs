using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CodeArchitects.Platform.Dapr.AspNetCore.DependencyInjection;

internal class DaprInfrastructureBuilder : IDaprInfrastructureBuilder, ILogger
{
  private readonly DaprConfiguration _daprConfiguration;
  private ILogger _logger;

  public DaprInfrastructureBuilder(IServiceCollection services, IConfiguration configuration, Func<ILogger, DaprConfiguration> daprConfigurationFactory)
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

  public IDisposable BeginScope<TState>(TState state)
  {
    return _logger.BeginScope(state);
  }

  public bool IsEnabled(LogLevel logLevel)
  {
    return _logger.IsEnabled(logLevel);
  }

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    _logger.Log(logLevel, eventId, state, exception, formatter);
  }
}
