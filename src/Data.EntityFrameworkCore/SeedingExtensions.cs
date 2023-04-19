using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

/// <summary>
/// Extension methods of <see cref="DbContext"/> for seeding the database.
/// </summary>
public static class SeedingExtensions
{
  /// <summary>
  /// Applies the specified seed to the database.
  /// </summary>
  /// <param name="context">The db context.</param>
  /// <param name="seed">The seed to apply.</param>
  public static void Seed(this DbContext context, DataSeed seed)
  {
    if (context is null)
      throw new ArgumentNullException(nameof(context));
    if (seed is null)
      throw new ArgumentNullException(nameof(seed));

    SeedCore(context, seed);
  }

  /// <summary>
  /// Applies the registered seed to the database.
  /// </summary>
  /// <param name="context">The db context.</param>
  public static void Seed(this DbContext context)
  {
    if (context is null)
      throw new ArgumentNullException(nameof(context));

    SeedCore(context, context.GetService<DataSeed>());
  }

  /// <summary>
  /// Applies the configured seed to the database.
  /// </summary>
  /// <remarks>
  /// A <see cref="DataSeed"/> type must be registered into the service provider.
  /// </remarks>
  /// <param name="services">The application service provider.</param>
  public static void Seed(this IServiceProvider services)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    DataSeed? seed = services.GetService<DataSeed>();

    if (seed is null)
    {
      ILogger<Seeder>? logger = services.GetService<ILogger<Seeder>>();
      logger?.LogWarning("No seed was specified.");
      return;
    }

    SeedingContextAttribute? seedingContextAttribute = seed.GetType().GetCustomAttribute<SeedingContextAttribute>(inherit: false);
    Type dbContextType = seedingContextAttribute is not null
      ? seedingContextAttribute.DbContextType
      : typeof(DbContext);

    using DbContext dbContext = (DbContext)services.GetRequiredService(dbContextType);

    SeedCore(dbContext, seed);
  }

  private static void SeedCore(DbContext dbContext, DataSeed seed)
  {
    Seeder seeder = new(dbContext);
    seeder.Apply(seed);
  }
}
