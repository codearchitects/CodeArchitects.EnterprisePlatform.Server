namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal interface IDataOptionsBuilderInfrastructure
{
  void AddOrUpdatePlugin<TPlugin>(TPlugin plugin)
    where TPlugin : class, IDataExtensionPlugin;
}
