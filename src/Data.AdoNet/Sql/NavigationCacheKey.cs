using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

internal readonly struct NavigationCacheKey : IEquatable<NavigationCacheKey>
{
  private readonly IEntityModel _root;
  private readonly IReadOnlyCollection<INavigation> _navigations;

  public NavigationCacheKey(IEntityModel root, IReadOnlyCollection<INavigation> navigations)
  {
    _root = root;
    _navigations = navigations;
  }

  public NavigationCacheKey(IEntityModel root)
  {
    _root = root;
    _navigations = Array.Empty<INavigation>();
  }

  public readonly bool Equals(NavigationCacheKey other)
  {
    return
      ReferenceEquals(other._root, _root) &&
      NavigationCollection.Equal(other._navigations, _navigations);
  }

  public readonly override bool Equals(object obj)
  {
    return obj is NavigationCacheKey other && Equals(other);
  }

  public readonly override int GetHashCode()
  {
    return _root.GetHashCode() + _navigations.Count;
  }
}
