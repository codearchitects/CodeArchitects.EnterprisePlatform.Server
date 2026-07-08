using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal abstract class CaepExtensionBase : IDbContextOptionsExtension
{
  private readonly IEnumerable<ICaepExtensionPlugin> _plugins;

  protected IEnumerable<ICaepExtensionPlugin> Plugins => _plugins;

  public CaepExtensionBase(IEnumerable<ICaepExtensionPlugin> plugins)
  {
    _plugins = plugins;
  }

  public abstract DbContextOptionsExtensionInfo Info { get; }

  public void ApplyServices(IServiceCollection services)
  {
    services.AddScoped<AggregateServiceProvider>();
    PluginServiceCollection pluginServices = new(services);

    foreach (ICaepExtensionPlugin plugin in _plugins)
    {
      plugin.ApplyServices(pluginServices);
    }
  }

  public void Validate(IDbContextOptions options)
  {
    // Nothing to validate
  }
}
