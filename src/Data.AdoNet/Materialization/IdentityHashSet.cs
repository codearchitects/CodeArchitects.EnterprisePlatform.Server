namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityHashSet<TEntity> : HashSet<TEntity>, IIdentityCollection<TEntity>
  where TEntity : class
{
  public IdentityHashSet(IEqualityComparer<TEntity> comparer)
    : base(comparer)
  {
  }

  public void AddEntity(TEntity? entity)
  {
    if (entity is null)
      return;

    Add(entity);
  }
}
