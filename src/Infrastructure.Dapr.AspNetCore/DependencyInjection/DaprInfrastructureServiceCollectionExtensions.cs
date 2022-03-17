using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;
using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.DependencyInjection;
using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IDaprInfrastructureBuilder"/>.
/// </summary>
public static class DaprInfrastructureServiceCollectionExtensions
{
  public const string DefaultComponentFolderPath = "/root/.caep/dapr/components";

  /// <summary>
  /// Adds basic Dapr infrastructure services to the <see cref="IServiceCollection"/>.
  /// </summary>
  /// <param name="services">The service collection to configure.</param>
  /// <param name="configuration">A Dapr configuration instance.</param>
  /// <returns>An <see cref="IDaprInfrastructureBuilder"/> that can be used to further configure the Dapr infrastructure services.</returns>
  public static IDaprInfrastructureBuilder AddDaprInfrastructure(this IServiceCollection services, DaprConfiguration? configuration = null)
  {
    if (configuration is not null)
    {
      services.AddSingleton(configuration);
    }
    return new DaprInfrastructureBuilder(services, configuration);
  }

  /// <summary>
  /// Adds basic Dapr infrastructure services to the <see cref="IServiceCollection"/>.
  /// </summary>
  /// <param name="services">The service collection to configure.</param>
  /// <param name="configure">A configuration building action.</param>
  /// <returns>An <see cref="IDaprInfrastructureBuilder"/> that can be used to further configure the Dapr infrastructure services.</returns>
  public static IDaprInfrastructureBuilder AddDaprInfrastructure(this IServiceCollection services, Action<IDaprConfigurationBuilder> configure)
  {
    ApplicationOptionsFactory applicationOptionsFactory = new ApplicationOptionsFactory();
    DaprConfigurationBuilder builder = new DaprConfigurationBuilder(applicationOptionsFactory);
    configure(builder);

    ServiceOptions? serviceOptions = builder.ServiceOptions;
    string componentFolderPath = serviceOptions?.ComponentFolderPath ?? DefaultComponentFolderPath;
    IFileProvider componentFolderProvider = Directory.Exists(componentFolderPath)
      ? new PhysicalFileProvider(componentFolderPath)
      : new NullFileProvider();

    DaprConfiguration configuration = builder
      .AddComponentFolderProvider(componentFolderProvider)
      .Build();

    services.AddSingleton(configuration);
    return new DaprInfrastructureBuilder(services, configuration);
  }
}
