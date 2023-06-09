using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal abstract class ListIterator<TSource1, TSource2, T> : IEnumerable<T>, IEnumerator<T>
{
  private int _index;

  public ListIterator()
  {
    _index = -1;
  }

  protected abstract IReadOnlyList<TSource1> List1 { get; }

  protected abstract IReadOnlyList<TSource2> List2 { get; }

  public T Current
  {
    get
    {
      Debug.Assert(_index >= 0 && _index < List1.Count);

      return OnCurrent(List1[_index], List2[_index]);
    }
  }

  protected abstract T OnCurrent(TSource1 element1, TSource2 element2);

  bool IEnumerator.MoveNext()
  {
    Debug.Assert(List1.Count == List2.Count);
    if (_index == List1.Count - 1)
      return false;

    _index++;
    return true;
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

  void IDisposable.Dispose()
  {
  }

  #region Not relevant

  [ExcludeFromCodeCoverage]
  object IEnumerator.Current => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  void IEnumerator.Reset() => throw new NotSupportedException();

  #endregion
}
