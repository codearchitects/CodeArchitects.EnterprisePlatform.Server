using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class EntityEqualityComparer<TEntity, TKey> : IEqualityComparer<TEntity>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private static EntityEqualityComparer<TEntity, TKey>? s_instance;

  private readonly IAccessor<TKey> _keyAccessor;

  public EntityEqualityComparer(IAccessor<TKey> keyAccessor)
  {
    _keyAccessor = keyAccessor;
  }

  public bool Equals(TEntity x, TEntity y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return EqualityComparer<TKey>.Default.Equals(_keyAccessor.Get(x), _keyAccessor.Get(y));
  }

  public int GetHashCode(TEntity obj)
  {
    return EqualityComparer<TKey>.Default.GetHashCode(_keyAccessor.Get(obj));
  }

  public static EntityEqualityComparer<TEntity, TKey> GetDefault(IEntityModel<TEntity, TKey> entity)
  {
    return s_instance ??= new(entity.PrimaryKey.Accessor);
  }
}
