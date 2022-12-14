using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public static class ModelExtensions
{
  public static IEntityModel GetEntity(this IDataModel model, Type entityType)
  {
    if (!model.TryGetEntity(entityType, out IEntityModel? entity))
      throw new KeyNotFoundException($"Could not find entity '{entityType.Name}'.");

    return entity;
  }

  public static IEntityModel<TEntity, TKey> GetEntity<TEntity, TKey>(this IDataModel model)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!model.TryGetEntity(out IEntityModel<TEntity, TKey>? entity))
      throw new KeyNotFoundException($"Could not find entity '{typeof(TEntity).Name}'.");

    return entity;
  }

  public static IAccessibleColumnModel GetColumn(this IEntityModel entity, ReadOnlySpan<char> name)
  {
    if (!entity.TryGetColumn(name, out IAccessibleColumnModel? column))
      throw new KeyNotFoundException($"Could not find column '{name.ToString()}' in entity '{entity.Type.Name}'.");

    return column;
  }

  public static IAccessibleNavigationModel GetNavigation(this IEntityModel entity, ReadOnlySpan<char> name)
  {
    if (!entity.TryGetNavigation(name, out IAccessibleNavigationModel? navigation))
      throw new KeyNotFoundException($"Could not find navigation '{name.ToString()}' in entity '{entity.Type.Name}'.");

    return navigation;
  }

  public static IPrimaryKeyColumnModel GetColumn(this IPrimaryKeyModel entity, ReadOnlySpan<char> name)
  {
    if (!entity.TryGetColumn(name, out IPrimaryKeyColumnModel? column))
      throw new KeyNotFoundException($"Could not find column '{name.ToString()}' in entity '{entity.Type.Name}'.");

    return column;
  }
}
