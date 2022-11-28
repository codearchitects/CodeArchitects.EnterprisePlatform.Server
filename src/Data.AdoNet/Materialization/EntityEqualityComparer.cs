namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal abstract class EntityEqualityComparer<TEntity, TKey> : IEqualityComparer<TEntity>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  protected abstract TKey GetKey(TEntity entity);

  public bool Equals(TEntity x, TEntity y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return EqualityComparer<TKey>.Default.Equals(GetKey(x), GetKey(y));
  }

  public int GetHashCode(TEntity obj)
  {
    return EqualityComparer<TKey>.Default.GetHashCode(GetKey(obj));
  }
}
