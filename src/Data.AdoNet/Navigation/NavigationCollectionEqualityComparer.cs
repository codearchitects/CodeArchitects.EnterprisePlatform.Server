namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class NavigationCollectionEqualityComparer : IEqualityComparer<IReadOnlyCollection<INavigation>>
{
  public static readonly NavigationCollectionEqualityComparer Instance = new();

  private NavigationCollectionEqualityComparer() { }

  public bool Equals(IReadOnlyCollection<INavigation>? x, IReadOnlyCollection<INavigation>? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Count != y.Count)
      return false;

    if (x.Count == 0)
      return true;

    using IEnumerator<INavigation> xEnumerator = x.GetEnumerator();
    using IEnumerator<INavigation> yEnumerator = y.GetEnumerator();

    while (xEnumerator.MoveNext() | yEnumerator.MoveNext()) // '|' on purpose
    {
      INavigation xCurrent = xEnumerator.Current;
      INavigation yCurrent = yEnumerator.Current;

      if (xCurrent.Model.Id != yCurrent.Model.Id)
        return false;

      if (!Equals(xCurrent.Children, yCurrent.Children))
        return false;
    }

    return true;
  }

  public int GetHashCode(IReadOnlyCollection<INavigation> obj)
  {
    if (obj.Count == 0)
      return 0;

    HashCode hashCode = new();
    foreach (INavigation navigation in obj)
    {
      hashCode.Add(navigation.Model.Id);

      if (navigation.Children.Count == 0)
        continue;

      hashCode.Add(GetHashCode(navigation.Children));
    }

    return hashCode.ToHashCode();
  }
}
