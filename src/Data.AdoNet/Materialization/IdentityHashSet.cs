namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityHashSet<T> : HashSet<T>, IIdentityCollection<T>
{
  public IdentityHashSet(IEqualityComparer<T> comparer)
    : base(comparer)
  {
  }

  public void AddEntity(T entity)
  {
    Add(entity);
  }
}
