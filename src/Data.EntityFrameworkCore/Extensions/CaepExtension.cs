using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class CaepExtension : CaepExtensionBase
{
  private readonly IEnumerable<ICaepExtensionPlugin> _plugins;

  public CaepExtension(IEnumerable<ICaepExtensionPlugin> plugins)
  {
    _plugins = plugins;
  }

  public override DbContextOptionsExtensionInfo Info => new CaepExtensionInfo(this);

  public override void ApplyServices(IServiceCollection services)
  {
    base.ApplyServices(services);
    services.AddScoped<AggregateServiceProvider>();

    PluginServiceCollection pluginServices = new(services);

    foreach (ICaepExtensionPlugin plugin in _plugins)
    {
      plugin.ApplyServices(pluginServices);
    }
  }

  private class CaepExtensionInfo : DbContextOptionsExtensionInfo
  {
    public CaepExtensionInfo(CaepExtension extension)
      : base(extension)
    {
    }

    public override bool IsDatabaseProvider => false;

    public override string LogFragment => "CodeArchitects:Caep";

    public override int GetServiceProviderHashCode()
    {
      return 0;
    }

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
    }

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
    {
      return true;
    }
  }
}
