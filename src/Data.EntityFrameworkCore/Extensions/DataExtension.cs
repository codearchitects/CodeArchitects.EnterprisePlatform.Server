using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class DataExtension : DataExtensionBase
{
  private readonly IEnumerable<IDataExtensionPlugin> _plugins;

  public DataExtension(IEnumerable<IDataExtensionPlugin> plugins)
  {
    _plugins = plugins;
  }

  public override DbContextOptionsExtensionInfo Info => new DataExtensionInfo(this);

  public override void ApplyServices(IServiceCollection services)
  {
    base.ApplyServices(services);
    PluginServiceCollection pluginServices = new(services);

    foreach (IDataExtensionPlugin plugin in _plugins)
    {
      plugin.ApplyServices(services, pluginServices);
    }
  }

  private class DataExtensionInfo : DbContextOptionsExtensionInfo
  {
    public DataExtensionInfo(DataExtension extension)
      : base(extension)
    {
    }

    public override bool IsDatabaseProvider => false;

    public override string LogFragment => "CodeArchitects:Data";

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
