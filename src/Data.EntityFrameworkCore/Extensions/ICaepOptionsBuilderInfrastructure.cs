namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal interface ICaepOptionsBuilderInfrastructure
{
  void AddOrUpdatePlugin<TPlugin>(TPlugin plugin)
    where TPlugin : class, ICaepExtensionPlugin;
}
