namespace CodeArchitects.Platform.Common.Collections;

internal static class Enumerate
{
  public static bool MoveNext<T1, T2>(IEnumerator<T1> enumerator1, IEnumerator<T2> enumerator2)
  {
    bool hasNext1 = enumerator1.MoveNext();
    bool hasNext2 = enumerator2.MoveNext();

    if (hasNext1 ^ hasNext2)
      throw new InvalidOperationException("Sequences contained a different number of elements.");

    return hasNext1 && hasNext2;
  }
}

