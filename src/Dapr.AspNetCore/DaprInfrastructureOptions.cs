using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using Dapr.Client;
using Microsoft.Extensions.Configuration;

namespace CodeArchitects.Platform.Dapr.AspNetCore;

internal class DaprInfrastructureOptions :
  IDaprInfrastructureOptionsBuilder,
  IConfiguredDaprInfrastructureOptionsBuilder
{
  public const string DefaultServiceConfigurationPath = "Caep:Dapr";

  private IConfigurationSection? _configurationSection;
  private DaprConfig? _config;
  private readonly HashSet<string> _componentsFolderPaths;

  public DaprInfrastructureOptions()
  {
    _componentsFolderPaths = new();
  }

  public IConfigurationSection ConfigurationSection
  {
    get => _configurationSection ?? throw new InvalidOperationException("Options were not configured");
    private set
    {
      _configurationSection = value;

      _config = new DaprConfig();
      _configurationSection.Bind(_config);
      if (_config.ComponentsFolderPath is { } componentsFolderPath)
      {
        _componentsFolderPaths.Add(componentsFolderPath);
      }
    }
  }

  public Action<DaprClientBuilder>? ConfigureDaprAction { get; private set; }
  
  public IEnumerable<string> ComponentsFolderPaths => _componentsFolderPaths;
  
  public DaprConfig Config => _config ?? throw new InvalidOperationException("Options were not configured");

  public IConfiguredDaprInfrastructureOptionsBuilder Configure(Action<DaprConfig> configure)
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    configure(Config);
    return this;
  }

  public IConfiguredDaprInfrastructureOptionsBuilder ConfigureDapr(Action<DaprClientBuilder> configure)
  {
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    ConfigureDaprAction = configure;
    return this;
  }

  public IConfiguredDaprInfrastructureOptionsBuilder AddComponentFolder(string path)
  {
    if (path is null)
      throw new ArgumentNullException(nameof(path));

    _componentsFolderPaths.Add(path);
    return this;
  }

  public IConfiguredDaprInfrastructureOptionsBuilder SetConfiguration(IConfiguration configuration)
  {
    if (configuration is null)
      throw new ArgumentNullException(nameof(configuration));

    ConfigurationSection = configuration.GetSection(DefaultServiceConfigurationPath);
    return this;
  }

  public IConfiguredDaprInfrastructureOptionsBuilder SetConfiguration(IConfigurationSection configurationSection)
  {
    if (configurationSection is null)
      throw new ArgumentNullException(nameof(configurationSection));

    ConfigurationSection = configurationSection;
    return this;
  }

  public IConfiguredDaprInfrastructureOptionsBuilder SetConfiguration(IConfiguration configuration, string key)
  {
    if (configuration is null)
      throw new ArgumentNullException(nameof(configuration));
    if (key is null)
      throw new ArgumentNullException(nameof(key));

    ConfigurationSection = configuration.GetSection(key);
    return this;
  }

  #region Deprecated methods

  public IConfiguredDaprInfrastructureOptionsBuilder SetServiceOptions(IConfigurationSection serviceConfiguration)
  {
    return SetConfiguration(serviceConfiguration);
  }

  public IConfiguredDaprInfrastructureOptionsBuilder SetServiceOptions(IConfiguration configuration)
  {
    return SetConfiguration(configuration);
  }

  #endregion
}
