namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityList<T> : List<T>, IIdentityCollection<T>, IIdentityList
{
  private readonly HashSet<T> _identitySet;

  public IdentityList(IEqualityComparer<T> comparer)
  {
    _identitySet = new(comparer);
  }

  public void AddEntity(T entity)
  {
    if (_identitySet.Contains(entity))
      return;

    _identitySet.Add(entity);
  }

  public void PopulateList()
  {
    AddRange(_identitySet);
  }
}
