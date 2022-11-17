namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

public class DataOptionsBuilder : IDataOptionsBuilderInfrastructure
{
  private readonly Dictionary<Type, IDataExtensionPlugin> _plugins;

  internal DataOptionsBuilder()
  {
    _plugins = new();
  }

  internal IEnumerable<IDataExtensionPlugin> Plugins => _plugins.Values;

  void IDataOptionsBuilderInfrastructure.AddOrUpdatePlugin<TPlugin>(TPlugin plugin)
  {
    ArgumentNullException.ThrowIfNull(plugin);

    _plugins[typeof(TPlugin)] = plugin;
  }
}
