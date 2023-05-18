namespace CodeArchitects.Platform.Data.MongoDB.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IMongoDBConfigurationBuilder"/>.
/// </summary>
public static class MongoDBConfigurationBuilderExtensions
{
  /// <summary>
  /// Specifies the seed type to use for seeding the database.
  /// </summary>
  /// <typeparam name="TDataSeed">The seed type.</typeparam>
  /// <param name="builder">The MongoDB configuration builder.</param>
  /// <returns>Returns an <see cref="IMongoDBConfigurationBuilder"/> for further configuration.</returns>
  /// <exception cref="ArgumentNullException"></exception>
  public static IMongoDBConfigurationBuilder UseSeed<TDataSeed>(this IMongoDBConfigurationBuilder builder)
    where TDataSeed : DataSeed
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.UseSeed(typeof(TDataSeed));
  }
}
