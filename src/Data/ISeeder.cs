namespace CodeArchitects.Platform.Data;

/// <summary>
/// Object used to add data to the seed.
/// </summary>
public interface ISeeder
{
  /// <summary>
  /// Adds a collection of entities to the seed.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <param name="entities">The entity collection.</param>
  void Seed<TEntity>(IEnumerable<TEntity> entities)
    where TEntity : class;
}
