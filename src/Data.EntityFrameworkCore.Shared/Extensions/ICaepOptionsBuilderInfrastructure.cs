namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Explicitly implemented by <see cref="CaepOptionsBuilder" /> to hide methods that are used by plug-in extension methods but not intended to be called by application developers.
/// </summary>
public interface ICaepOptionsBuilderInfrastructure
{
  /// <summary>
  /// Adds the given plug-in to the options. If an existing plug-in of the same type already exists, it will be replaced.
  /// </summary>
  /// <typeparam name="TPlugin">The type of plug-in to be added.</typeparam>
  /// <param name="plugin">The plug-in to be added.</param>
  void AddOrUpdatePlugin<TPlugin>(TPlugin plugin)
    where TPlugin : class, ICaepExtensionPlugin;
}
