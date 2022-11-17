using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

public interface IDataExtensionPlugin
{
  void ApplyServices(IServiceCollection services, IPluginServiceCollection pluginServices);
}
