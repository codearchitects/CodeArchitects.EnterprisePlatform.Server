using System.Diagnostics;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityList<TEntity> : List<TEntity>, IIdentityCollection
  where TEntity : class
{
  private readonly HashSet<TEntity> _identitySet;

  public IdentityList(IEqualityComparer<TEntity> comparer)
  {
    _identitySet = new(comparer);
  }

  public void AddEntity(object entity)
  {
    if (_identitySet.Contains(entity))
      return;

    _identitySet.Add((TEntity)entity);
  }

  public void Populate()
  {
    Debug.Assert(Count == 0);
    
    AddRange(_identitySet);
  }
}
