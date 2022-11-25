namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal static class NavigationCollection
{
  public static bool Equal(IReadOnlyCollection<INavigation> left, IReadOnlyCollection<INavigation> right)
  {
    if (left.Count != right.Count)
      return false;

    using IEnumerator<INavigation> leftEnumerator = left.GetEnumerator();
    using IEnumerator<INavigation> rightEnumerator = right.GetEnumerator();

    while (leftEnumerator.MoveNext() | rightEnumerator.MoveNext()) // '|' on purpose
    {
      if (!leftEnumerator.Current.Equals(rightEnumerator.Current))
        return false;
    }

    return true;
  }
}
