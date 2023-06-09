namespace CodeArchitects.Platform.Common.Collections;

internal static class Enumerate
{
  public static bool MoveNext<T1, T2>(IEnumerator<T1> enumerator1, IEnumerator<T2> enumerator2)
    => enumerator1.MoveNext() | enumerator2.MoveNext(); // '|' on purpose
}

