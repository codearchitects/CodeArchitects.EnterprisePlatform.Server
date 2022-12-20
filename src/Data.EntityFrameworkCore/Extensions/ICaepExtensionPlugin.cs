namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Interface that a plugin for the CAEP extension must implement.
/// </summary>
public interface ICaepExtensionPlugin
{
  /// <summary>
  /// Applies the plugin's services.
  /// </summary>
  /// <param name="services">The plugin service collection.</param>
  void ApplyServices(IPluginServiceCollection services);
}
