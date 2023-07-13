using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal abstract class ListIterator<TSource, T> : IEnumerable<T>, IEnumerator<T>
{
  private int _index;

  public ListIterator()
  {
    _index = -1;
  }

  protected abstract IReadOnlyList<TSource> List { get; }

  public T Current
  {
    get
    {
      Debug.Assert(_index >= 0 && _index < List.Count);

      return OnCurrent(List[_index]);
    }
  }

  protected abstract T OnCurrent(TSource element);

  bool IEnumerator.MoveNext()
  {
    if (_index == List.Count - 1)
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
