using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal struct NavigationCacheKey : IEquatable<NavigationCacheKey>
{
  private readonly IEntityModel _root;
  private readonly IReadOnlyList<int> _navigationIds;

  public NavigationCacheKey(IEntityModel root, IReadOnlyList<int> navigationIds)
  {
    _root = root;
    _navigationIds = navigationIds;
  }

  public NavigationCacheKey(IEntityModel root)
  {
    _root = root;
    _navigationIds = Array.Empty<int>();
  }

  public bool Equals(NavigationCacheKey other)
  {
    if (!ReferenceEquals(_root, other._root))
      return false;

    IReadOnlyList<int> navigationIds = _navigationIds;
    IReadOnlyList<int> otherNavigationIds = other._navigationIds;

    if (navigationIds.Count != otherNavigationIds.Count)
      return false;

    for (int i = 0; i < navigationIds.Count; i++)
    {
      if (navigationIds[i] != otherNavigationIds[i])
        return false;
    }

    return true;
  }

  public override bool Equals(object obj)
  {
    return obj is NavigationCacheKey other && Equals(other);
  }

  public override int GetHashCode()
  {
    return _root.GetHashCode() + _navigationIds.Count;
  }
}
