using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using Dapr.Client;
using Microsoft.Extensions.Configuration;

namespace CodeArchitects.Platform.Dapr.AspNetCore;

public interface IDaprInfrastructureOptionsBuilder
{
  IConfiguredDaprInfrastructureOptionsBuilder SetConfiguration(IConfiguration configuration);
  IConfiguredDaprInfrastructureOptionsBuilder SetConfiguration(IConfigurationSection configurationSection);
  IConfiguredDaprInfrastructureOptionsBuilder SetConfiguration(IConfiguration configuration, string key);

  #region Deprecated methods
  [Obsolete($"This method will be removed in next release. Use the {nameof(SetConfiguration)} method instead.")]
  IConfiguredDaprInfrastructureOptionsBuilder SetServiceOptions(IConfigurationSection serviceConfiguration);

  [Obsolete($"This method will be removed in next release. Use the {nameof(SetConfiguration)} method instead.")]
  IConfiguredDaprInfrastructureOptionsBuilder SetServiceOptions(IConfiguration configuration);
  #endregion
}

public interface IConfiguredDaprInfrastructureOptionsBuilder
{
  IConfiguredDaprInfrastructureOptionsBuilder Configure(Action<DaprConfig> configure);
  IConfiguredDaprInfrastructureOptionsBuilder ConfigureDapr(Action<DaprClientBuilder> configure);
  IConfiguredDaprInfrastructureOptionsBuilder AddComponentFolder(string path);
}
