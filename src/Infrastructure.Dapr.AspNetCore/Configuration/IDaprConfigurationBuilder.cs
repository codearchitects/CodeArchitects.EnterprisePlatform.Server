using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using Microsoft.Extensions.Configuration;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;

/// <summary>
/// Interface for configuring the Dapr infrastructure.
/// </summary>
public interface IDaprConfigurationBuilder
{
  /// <summary>
  /// Uses the Dapr service options (see <see cref="ServiceOptions"/>) defined in the given configuration section.
  /// </summary>
  /// <param name="serviceConfiguration">The configuration section that contains the Dapr service options.</param>
  /// <returns>The same builder so it can be further configured.</returns>
  IDaprConfigurationBuilder AddServiceOptions(IConfigurationSection serviceConfiguration);

  /// <summary>
  /// Uses the Dapr service options (see <see cref="ServiceOptions"/>) defined in the <see cref="Constants.DefaultDaprConfigurationSectionKey"/> section of the given configuration.
  /// </summary>
  /// <param name="configuration">The application configuration instance.</param>
  /// <returns>The same builder so it can be further configured.</returns>
  IDaprConfigurationBuilder AddServiceOptions(IConfiguration configuration);
}
