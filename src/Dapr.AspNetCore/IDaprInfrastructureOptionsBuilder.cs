using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using Dapr.Client;
using Microsoft.Extensions.Configuration;

namespace CodeArchitects.Platform.Dapr.AspNetCore;

/// <summary>
/// A fluent builder that can be used to configure Dapr infrastructure options.
/// </summary>
public interface IDaprInfrastructureOptionsBuilder
{
  /// <summary>
  /// Uses the given configuration object and the default key 'Caep:Dapr' to retrieve the Dapr infrastructure configuration section.
  /// </summary>
  /// <param name="configuration">The configuration object.</param>
  /// <returns>The builder.</returns>
  IConfiguredDaprInfrastructureOptionsBuilder SetConfiguration(IConfiguration configuration);

  /// <summary>
  /// Uses the given configuration section as the Dapr infrastructure configuration section.
  /// </summary>
  /// <param name="configurationSection">The configuration section.</param>
  /// <returns>The builder.</returns>
  IConfiguredDaprInfrastructureOptionsBuilder SetConfiguration(IConfigurationSection configurationSection);

  /// <summary>
  /// Uses the given configuration object and the the given key to retrieve the Dapr infrastructure configuration section.
  /// </summary>
  /// <param name="configuration">The configuration object.</param>
  /// <param name="key">The path of the Dapr infrastructure configuration section.</param>
  /// <returns>The builder.</returns>
  IConfiguredDaprInfrastructureOptionsBuilder SetConfiguration(IConfiguration configuration, string key);

  #region Deprecated methods
  [Obsolete($"This method will be removed in next release. Use the {nameof(SetConfiguration)} method instead.")]
  IConfiguredDaprInfrastructureOptionsBuilder SetServiceOptions(IConfigurationSection serviceConfiguration);

  [Obsolete($"This method will be removed in next release. Use the {nameof(SetConfiguration)} method instead.")]
  IConfiguredDaprInfrastructureOptionsBuilder SetServiceOptions(IConfiguration configuration);
  #endregion
}

/// <summary>
/// A fluent builder that can be used to configure Dapr infrastructure options.
/// </summary>
public interface IConfiguredDaprInfrastructureOptionsBuilder
{
  /// <summary>
  /// Further configures the Dapr configuration object.
  /// </summary>
  /// <param name="configure">The configuration action.</param>
  /// <returns>The builder.</returns>
  IConfiguredDaprInfrastructureOptionsBuilder Configure(Action<DaprConfig> configure);

  /// <summary>
  /// Configures the Dapr client.
  /// </summary>
  /// <param name="configure">The configuration action.</param>
  /// <returns>The builder.</returns>
  IConfiguredDaprInfrastructureOptionsBuilder ConfigureDaprClient(Action<DaprClientBuilder> configure);

  /// <summary>
  /// Adds a location where to read component files from.
  /// </summary>
  /// <param name="path">The path to the folder.</param>
  /// <returns>The builder.</returns>
  IConfiguredDaprInfrastructureOptionsBuilder AddComponentFolder(string path);
}
