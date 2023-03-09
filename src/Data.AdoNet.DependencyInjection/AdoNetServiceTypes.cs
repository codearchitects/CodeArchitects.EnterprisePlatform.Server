using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.AdoNet.DependencyInjection;

/// <summary>
/// Indicates what services to scan for when calling <see cref="IAdoNetConfigurationBuilderWithProvider.ScanAssemblyForServices(System.Reflection.Assembly, AdoNetServiceTypes)"/>.
/// </summary>
[Flags]
public enum AdoNetServiceTypes
{
  /// <summary>
  /// Do not look for any services.
  /// </summary>
  None = 0,

  /// <summary>
  /// Look for model configuration classes.
  /// </summary>
  ModelConfiguration = 1,

  /// <summary>
  /// Look for command interceptors.
  /// </summary>
  CommandInterceptors = 2,

  /// <summary>
  /// Look for data seed.
  /// </summary>
  DataSeed = 4,

  /// <summary>
  /// Look for all types of services.
  /// </summary>
  All = ModelConfiguration | CommandInterceptors | DataSeed
}
