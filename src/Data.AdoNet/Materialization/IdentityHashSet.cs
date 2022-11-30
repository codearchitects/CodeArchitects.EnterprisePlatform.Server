namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityHashSet<TEntity> : HashSet<TEntity>, IIdentityCollection
  where TEntity : class
{
  public IdentityHashSet(IEqualityComparer<TEntity> comparer)
    : base(comparer)
  {
  }

  public void AddEntity(object? entity)
  {
    if (entity is null)
      return;

    Add((TEntity)entity);
  }
}
