using CodeArchitects.Platform.Dapr.AspNetCore.Configuration;
using CodeArchitects.Platform.Dapr.AspNetCore.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class DaprServiceCollectionExtensions
{
  public const string DefaultServiceConfigurationPath = "Caep:Dapr";

  /// <summary>
  /// Adds basic Dapr infrastructure services to the <see cref="IServiceCollection"/>.
  /// </summary>
  /// <param name="services">The service collection to configure.</param>
  /// <param name="configuration">A configuration instance.</param>
  /// <returns>An <see cref="IDaprInfrastructureBuilder"/> that can be used to further configure the Dapr infrastructure services.</returns>
  public static IDaprInfrastructureBuilder AddDaprInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (configuration is null)
      throw new ArgumentNullException(nameof(configuration));

    return services.AddDaprInfrastructureCore(configuration, DefaultServiceConfigurationPath);
  }

  /// <summary>
  /// Adds basic Dapr infrastructure services to the <see cref="IServiceCollection"/>.
  /// </summary>
  /// <param name="services">The service collection to configure.</param>
  /// <param name="configuration">A configuration instance.</param>
  /// <param name="key">The configuration section key.</param>
  /// <returns>An <see cref="IDaprInfrastructureBuilder"/> that can be used to further configure the Dapr infrastructure services.</returns>
  public static IDaprInfrastructureBuilder AddDaprInfrastructure(this IServiceCollection services, IConfiguration configuration, string key)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (configuration is null)
      throw new ArgumentNullException(nameof(configuration));

    return services.AddDaprInfrastructureCore(configuration, key);
  }

  private static IDaprInfrastructureBuilder AddDaprInfrastructureCore(this IServiceCollection services, IConfiguration configuration, string key)
  {
    IConfigurationSection configurationSection = configuration.GetSection(key);
    Func<ILogger, DaprConfiguration> daprConfigurationFactory = delegate (ILogger logger)
    {
      return DaprConfiguration.Create(new ComponentReader(), configurationSection, logger);
    };

    DaprOptions options = new DaprOptions();
    configurationSection.Bind(options);

    var builder = new DaprInfrastructureBuilder(services, configurationSection, daprConfigurationFactory);

    if (options.ComponentsFolderPath is string componentsFolderPath && Directory.Exists(componentsFolderPath))
    {
      builder.DaprConfigurationBuilder.AddComponents(new PhysicalFileProvider(componentsFolderPath));
    }

    return builder;
  }
}
