using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Extension methods for model interfaces.
/// </summary>
[Experimental]
public static class ModelExtensions
{
  /// <summary>
  /// Gets the entity with the specified type.
  /// </summary>
  /// <param name="model">The data model.</param>
  /// <param name="entityType">The type of the entity to get.</param>
  /// <returns>The entity, if found.</returns>
  public static IEntityModel GetEntity(this IDataModel model, Type entityType)
  {
    if (!model.TryGetEntity(entityType, out IEntityModel? entity))
      throw new KeyNotFoundException($"Could not find entity '{entityType.Name}'.");

    return entity;
  }

  /// <summary>
  /// Gets the entity with the specified type and primary key type.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity to get.</typeparam>
  /// <typeparam name="TKey">The type of the primary key of the entity.</typeparam>
  /// <param name="model">The data model.</param>
  /// <returns>The entity, if found.</returns>
  public static IEntityModel<TEntity, TKey> GetEntity<TEntity, TKey>(this IDataModel model)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!model.TryGetEntity(out IEntityModel<TEntity, TKey>? entity))
      throw new KeyNotFoundException($"Could not find entity '{typeof(TEntity).Name}'.");

    return entity;
  }

  /// <summary>
  /// Gets the column with the specified name.
  /// </summary>
  /// <param name="entity">The entity model.</param>
  /// <param name="name">The name of the column to get.</param>
  /// <returns>The column, if found.</returns>
  public static IAccessibleColumnModel GetColumn(this IEntityModel entity, ReadOnlySpan<char> name)
  {
    if (!entity.TryGetColumn(name, out IAccessibleColumnModel? column))
      throw new KeyNotFoundException($"Could not find column '{name.ToString()}' in entity '{entity.Type.Name}'.");

    return column;
  }

  /// <summary>
  /// Gets the navigation with the specified name.
  /// </summary>
  /// <param name="entity">The entity model.</param>
  /// <param name="name">The name of the navigation to get.</param>
  /// <returns>The navigation, if found.</returns>
  public static IAccessibleNavigationModel GetNavigation(this IEntityModel entity, ReadOnlySpan<char> name)
  {
    if (!entity.TryGetNavigation(name, out IAccessibleNavigationModel? navigation))
      throw new KeyNotFoundException($"Could not find navigation '{name.ToString()}' in entity '{entity.Type.Name}'.");

    return navigation;
  }

  /// <summary>
  /// Gets the primary key column with the specified name.
  /// </summary>
  /// <param name="key">The primary key model.</param>
  /// <param name="name">The name of the column to get.</param>
  /// <returns>The column, if found.</returns>
  public static IPrimaryKeyColumnModel GetColumn(this IPrimaryKeyModel key, ReadOnlySpan<char> name)
  {
    if (!key.TryGetColumn(name, out IPrimaryKeyColumnModel? column))
      throw new KeyNotFoundException($"Could not find column '{name.ToString()}' in entity '{key.Type.Name}'.");

    return column;
  }
}
