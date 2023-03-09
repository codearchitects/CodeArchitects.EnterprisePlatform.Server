namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityHashSet<TEntity> : HashSet<TEntity>, IIdentityCollection
  where TEntity : class
{
  public IdentityHashSet(IEqualityComparer<TEntity> comparer)
    : base(comparer)
  {
  }

  public void AddEntity(object entity)
  {
    Add((TEntity)entity);
  }

  public void Populate()
  {
    // The collection is already populated
  }
}
