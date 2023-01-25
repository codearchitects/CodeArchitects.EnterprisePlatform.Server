namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Interface that a plug-in for the CAEP extension must implement.
/// </summary>
public interface ICaepExtensionPlugin
{
  /// <summary>
  /// Applies the plug-in's services.
  /// </summary>
  /// <param name="services">The plug-in service collection.</param>
  void ApplyServices(IPluginServiceCollection services);
}
