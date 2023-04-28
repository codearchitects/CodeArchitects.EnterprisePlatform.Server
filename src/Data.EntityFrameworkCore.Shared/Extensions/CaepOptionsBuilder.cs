namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Class used to configure the CAEP extension.
/// </summary>
public class CaepOptionsBuilder : ICaepOptionsBuilderInfrastructure
{
  private readonly Dictionary<Type, ICaepExtensionPlugin> _plugins;

  internal CaepOptionsBuilder()
  {
    _plugins = new();
  }

  internal IEnumerable<ICaepExtensionPlugin> Plugins => _plugins.Values;

  void ICaepOptionsBuilderInfrastructure.AddOrUpdatePlugin<TPlugin>(TPlugin plugin)
  {
    if (plugin is null)
      throw new ArgumentNullException(nameof(plugin));

    _plugins[typeof(TPlugin)] = plugin;
  }
}
