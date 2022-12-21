namespace CodeArchitects.Platform.Data;

/// <summary>
/// Extension methods for <see cref="ISeeder"/>.
/// </summary>
public static class SeederExtensions
{
  /// <summary>
  /// Adds a collection of entities to the seed.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <param name="seeder">The seeder.</param>
  /// <param name="entities">The entity collection.</param>
  public static void Seed<TEntity>(this ISeeder seeder, params TEntity[] entities)
    where TEntity : class
  {
    seeder.Seed(entities);
  }
}

