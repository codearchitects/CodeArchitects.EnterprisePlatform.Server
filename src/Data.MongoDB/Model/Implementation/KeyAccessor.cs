using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

/// <summary>
/// Reads the key value of an entity through a compiled accessor, cached per entity type.
/// </summary>
internal static class KeyAccessor
{
  private static readonly ConcurrentDictionary<Type, Delegate> s_accessors = new();

  public static TKey Get<TEntity, TKey>(IEntityModel model, TEntity entity)
    where TEntity : class
  {
    if (entity is null)
      throw new ArgumentNullException(nameof(entity));

    return GetAccessor<TEntity, TKey>(model).Invoke(entity);
  }

  private static Func<TEntity, TKey> GetAccessor<TEntity, TKey>(IEntityModel model)
    where TEntity : class
  {
    // Reflection is paid once per entity type: every later read goes through the compiled delegate.
    return (Func<TEntity, TKey>)s_accessors.GetOrAdd(typeof(TEntity), static (_, state) =>
    {
      ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");
      Expression key = Expression.Property(parameter, state.Key.Name);

      // The key property may be declared as a nullable value type while TKey is not.
      if (key.Type != typeof(TKey))
      {
        key = Expression.Convert(key, typeof(TKey));
      }

      return Expression.Lambda<Func<TEntity, TKey>>(key, parameter).Compile();
    }, model);
  }
}
