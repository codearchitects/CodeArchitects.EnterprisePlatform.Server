using CodeArchitects.Platform.Common.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Entry points for applying the configured <see cref="DataSeed"/> instances.
/// </summary>
[Experimental]
public static class SeedingExtensions
{
  /// <summary>
  /// Applies every registered <see cref="DataSeed"/>. Does nothing when none is registered.
  /// </summary>
  /// <param name="services">The application services.</param>
  /// <remarks>
  /// Meant to be called once at start-up. Seeding is idempotent per collection: a seed is only
  /// applied to a collection that is empty.
  /// </remarks>
  public static void SeedMongo(this IServiceProvider services)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    using IServiceScope scope = services.CreateScope();

    if (!TryGetSeeds(scope, out DataSeed[] seeds))
      return;

    scope.ServiceProvider.GetRequiredService<Seeder>().Apply(seeds);
  }

  /// <summary>
  /// Applies every registered <see cref="DataSeed"/>. Does nothing when none is registered.
  /// </summary>
  /// <param name="services">The application services.</param>
  /// <param name="cancellationToken">A token to cancel the operation.</param>
  public static async Task SeedMongoAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    using IServiceScope scope = services.CreateScope();

    if (!TryGetSeeds(scope, out DataSeed[] seeds))
      return;

    await scope.ServiceProvider.GetRequiredService<Seeder>().ApplyAsync(seeds, cancellationToken);
  }

  private static bool TryGetSeeds(IServiceScope scope, out DataSeed[] seeds)
  {
    seeds = scope.ServiceProvider.GetServices<DataSeed>().ToArray();

    if (seeds.Length > 0)
      return true;

    scope.ServiceProvider.GetService<ILogger<Seeder>>()?
      .LogWarning("No DataSeed is registered: seeding was skipped.");

    return false;
  }
}
