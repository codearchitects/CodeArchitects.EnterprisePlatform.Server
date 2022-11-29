using System.Diagnostics;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityList<TEntity> : List<TEntity>, IIdentityCollection<TEntity>, IIdentityList
  where TEntity : class
{
  private readonly HashSet<TEntity> _identitySet;

  public IdentityList(IEqualityComparer<TEntity> comparer)
  {
    _identitySet = new(comparer);
  }

  public void AddEntity(TEntity? entity)
  {
    if (entity is null)
      return;

    if (_identitySet.Contains(entity))
      return;

    _identitySet.Add(entity);
  }

  public void PopulateList()
  {
    Debug.Assert(Count == 0);
    
    AddRange(_identitySet);
  }
}
