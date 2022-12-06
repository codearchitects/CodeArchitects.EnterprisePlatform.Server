using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class EntityEqualityComparer<TEntity, TKey> : IEqualityComparer<TEntity>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private static EntityEqualityComparer<TEntity, TKey>? s_instance;

  private readonly Getter<TKey> _keyGetter;

  public EntityEqualityComparer(Getter<TKey> keyGetter)
  {
    _keyGetter = keyGetter;
  }

  public bool Equals(TEntity? x, TEntity? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return EqualityComparer<TKey>.Default.Equals(_keyGetter(x), _keyGetter(y));
  }

  public int GetHashCode(TEntity obj)
  {
    return EqualityComparer<TKey>.Default.GetHashCode(_keyGetter(obj));
  }

  public static EntityEqualityComparer<TEntity, TKey> GetDefault(IEntityModel<TEntity, TKey> entity)
  {
    return s_instance ??= new(entity.PrimaryKey.GetValue);
  }
}
