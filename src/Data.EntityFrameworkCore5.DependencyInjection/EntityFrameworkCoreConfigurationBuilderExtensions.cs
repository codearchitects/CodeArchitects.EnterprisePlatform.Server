using CodeArchitects.Platform.Data;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IEntityFrameworkCoreConfigurationBuilder"/>.
/// </summary>
public static class EntityFrameworkCoreConfigurationBuilderExtensions
{
  /// <summary>
  /// Specifies the seed type to use for seeding the database.
  /// </summary>
  /// <typeparam name="TDataSeed">The seed type.</typeparam>
  /// <param name="builder">The Entity Framework Core configuration builder.</param>
  /// <returns>An <see cref="IEntityFrameworkCoreConfigurationBuilder"/> for further configuration.</returns>
  public static IEntityFrameworkCoreConfigurationBuilder UseSeed<TDataSeed>(this IEntityFrameworkCoreConfigurationBuilder builder)
    where TDataSeed : DataSeed
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.UseSeed(typeof(TDataSeed));
  }
}
