using CodeArchitects.Platform.Data;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IMongoDBConfigurationBuilderWithDatabase"/>.
/// </summary>
public static class MongoDBConfigurationBuilderExtensions
{
  /// <summary>
  /// Specifies the seed type to use for seeding the database.
  /// </summary>
  /// <typeparam name="TDataSeed">The seed type.</typeparam>
  /// <param name="builder">The MongoDB configuration builder.</param>
  /// <returns>Returns an <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further configuration.</returns>
  /// <exception cref="ArgumentNullException"></exception>
  public static IMongoDBConfigurationBuilderWithDatabase UseSeed<TDataSeed>(this IMongoDBConfigurationBuilderWithDatabase builder)
    where TDataSeed : DataSeed
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.UseSeed(typeof(TDataSeed));
  }

  /// <summary>
  /// Registers a single entity type, whether or not it carries a discovery attribute.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <param name="builder">The MongoDB configuration builder.</param>
  /// <returns>Returns an <see cref="IMongoDBConfigurationBuilderWithDatabase"/> for further configuration.</returns>
  /// <exception cref="ArgumentNullException"></exception>
  public static IMongoDBConfigurationBuilderWithDatabase AddEntity<TEntity>(this IMongoDBConfigurationBuilderWithDatabase builder)
    where TEntity : class
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.AddEntity(typeof(TEntity));
  }
}
