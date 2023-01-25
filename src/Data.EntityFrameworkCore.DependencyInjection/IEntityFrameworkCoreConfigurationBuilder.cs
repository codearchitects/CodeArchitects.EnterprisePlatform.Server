using CodeArchitects.Platform.Data;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// An object used to configure the Entity Framework Core context and services.
/// </summary>
public interface IEntityFrameworkCoreConfigurationBuilder
{
  /// <summary>
  /// Specifies the seed type to use for seeding the database.
  /// </summary>
  /// <param name="seedType">The seed type. It must extend <see cref="DataSeed"/>.</param>
  /// <returns>An <see cref="IEntityFrameworkCoreConfigurationBuilder"/> for further configuration.</returns>
  IEntityFrameworkCoreConfigurationBuilder UseSeed(Type seedType);
}
