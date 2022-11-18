namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

public interface IDataExtensionPlugin
{
  void ApplyServices(IPluginServiceCollection services);
}
