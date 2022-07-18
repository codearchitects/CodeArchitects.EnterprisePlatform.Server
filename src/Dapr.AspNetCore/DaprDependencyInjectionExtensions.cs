using CodeArchitects.Platform.Dapr.AspNetCore;
using CodeArchitects.Platform.Dapr.AspNetCore.Services;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Methods for adding Dapr infrastructure via dependency injection.
/// </summary>
public static class DaprDependencyInjectionExtensions
{
  /// <summary>
  /// Adds basic Dapr infrastructure services.
  /// </summary>
  /// <param name="services">The service collection to configure.</param>
  /// <param name="configure">Action that configures the infrastructure options.</param>
  /// <returns>An <see cref="IDaprInfrastructureBuilder"/> that can be used to further configure the Dapr infrastructure services.</returns>
  public static IDaprInfrastructureBuilder AddDaprInfrastructure(this IServiceCollection services, Func<IDaprInfrastructureOptionsBuilder, IConfiguredDaprInfrastructureOptionsBuilder> configure)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    DaprInfrastructureOptions options = new();
    configure(options);

    services.AddDaprClient(options.ConfigureDaprAction);

    return CreateInfrastructureBuilder(services, options);
  }

  /// <summary>
  /// Adds basic Dapr infrastructure services.
  /// </summary>
  /// <param name="builder">The builder to configure.</param>
  /// <param name="configure">Action that configures the infrastructure options.</param>
  /// <returns>An <see cref="IDaprInfrastructureBuilder"/> that can be used to further configure the Dapr infrastructure services.</returns>
  public static IDaprInfrastructureBuilder AddDaprInfrastructure(this IMvcBuilder builder, Func<IDaprInfrastructureOptionsBuilder, IConfiguredDaprInfrastructureOptionsBuilder> configure)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    DaprInfrastructureOptions options = new();
    configure(options);

    builder.AddDapr(options.ConfigureDaprAction);

    return CreateInfrastructureBuilder(builder.Services, options);
  }

  private static IDaprInfrastructureBuilder CreateInfrastructureBuilder(IServiceCollection services, DaprInfrastructureOptions options)
  {
    DaprInfrastructureBuilder builder = DaprInfrastructureBuilder.Create(services, options.ConfigurationSection);

    foreach (string componentsFolderPath in options.ComponentsFolderPaths)
    {
      builder.ComponentAccessor.AddComponents(new PhysicalFileProvider(componentsFolderPath));
    }

    builder.DaprServices.AddService(options.Config);

    builder.Services.AddSingleton<IDaprInfrastructureServiceProvider>(builder.DaprServices);

    return builder;
  }
}
