using CodeArchitects.Platform.Data.MongoDB.Model;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.Filters;

/// <summary>
/// Builds key filters directly against the <c>_id</c> BSON element, without going through
/// the LINQ translator.
/// </summary>
internal sealed class FilterProvider : IFilterProvider
{
  public FilterDefinition<TEntity> ById<TEntity, TKey>(IEntityModel model, TKey key)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (model is null)
      throw new ArgumentNullException(nameof(model));

    return Builders<TEntity>.Filter.Eq(model.Key.ElementName, key);
  }

  public FilterDefinition<TEntity> ByEntity<TEntity, TKey>(IEntityModel model, TEntity entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (model is null)
      throw new ArgumentNullException(nameof(model));
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    return ById<TEntity, TKey>(model, KeyAccessor.Get<TEntity, TKey>(model, entity));
  }
}
