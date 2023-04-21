using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.MongoDB.DependencyInjection;

/// <summary>
/// Methods for adding the MongoDB data context to the application services.
/// </summary>
public static class DataMongoDBServiceCollectionExtensions
{
  /// <summary>
  /// Injects the services needed to support the MongoDB data context.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configure">The MongoDB configuration.</param>
  /// <returns>The same <see cref="IServiceCollection"/> instance.</returns>
  /// <exception cref="ArgumentNullException"></exception>
  public static IServiceCollection AddData(this IServiceCollection services, Func<IMongoDBConfigurationBuilder, IMongoDBConfigurationBuilderWithDatabase> configure)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (configure is null)
      throw new ArgumentNullException(nameof(configure));

    MongoDBConfigurationBuilder builder = new();
    configure(builder);

    builder.AddServices(services);

    return services;
  }
}
