using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;

/// <summary>
/// Implementation of <see cref="IDaprConfigurationBuilder"/>.
/// </summary>
internal class DaprConfigurationBuilder : IDaprConfigurationBuilder
{
  private readonly IApplicationOptionsFactory _applicationOptionsFactory;
  private readonly List<IFileProvider> _componentFolderProviders;

  public DaprConfigurationBuilder(IApplicationOptionsFactory applicationOptionsFactory)
  {
    _applicationOptionsFactory = applicationOptionsFactory;
    _componentFolderProviders = new List<IFileProvider>();
  }

  public ServiceOptions? ServiceOptions { get; private set; }

  public IDaprConfigurationBuilder AddServiceOptions(IConfigurationSection serviceConfiguration)
  {
    return AddServiceOptionsCore(serviceConfiguration);
  }

  public IDaprConfigurationBuilder AddServiceOptions(IConfiguration configuration)
  {
    return AddServiceOptionsCore(configuration.GetSection("Caep:Dapr"));
  }

  public DaprConfigurationBuilder AddComponentFolderProvider(IFileProvider provider)
  {
    _componentFolderProviders.Add(provider);
    return this;
  }

  private IDaprConfigurationBuilder AddServiceOptionsCore(IConfigurationSection serviceConfiguration)
  {
    ServiceOptions = new ServiceOptions();
    serviceConfiguration.Bind(ServiceOptions);
    return this;
  }

  /// <summary>
  /// Builds a <see cref="DaprConfiguration"/> instance.
  /// </summary>
  /// <returns>The built instance.</returns>
  public DaprConfiguration Build()
  {
    CompositeFileProvider provider = new CompositeFileProvider(_componentFolderProviders);

    return new DaprConfiguration
    {
      Service = ServiceOptions,
      Application = _applicationOptionsFactory.FromFileProvider(provider)
    };
  }
}
