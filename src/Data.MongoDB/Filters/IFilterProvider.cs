using CodeArchitects.Platform.Data.MongoDB.Model;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.Filters;

/// <summary>
/// Builds the filters used to address a single document by its key.
/// </summary>
internal interface IFilterProvider
{
  /// <summary>
  /// Builds a filter matching the document whose <c>_id</c> equals <paramref name="key"/>.
  /// </summary>
  FilterDefinition<TEntity> ById<TEntity, TKey>(IEntityModel model, TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  /// <summary>
  /// Builds a filter matching the document whose <c>_id</c> equals the key of <paramref name="entity"/>.
  /// </summary>
  FilterDefinition<TEntity> ByEntity<TEntity, TKey>(IEntityModel model, TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
