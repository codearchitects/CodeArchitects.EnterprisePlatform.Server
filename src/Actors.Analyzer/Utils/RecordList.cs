using System.Collections;

namespace CodeArchitects.Platform.Actors.Analyzer.Utils;

internal readonly struct RecordList<T> : IReadOnlyList<T>, IEquatable<RecordList<T>>
  where T : IEquatable<T>
{
  private readonly IReadOnlyList<T> _list;

  public RecordList(IReadOnlyList<T> list)
  {
    _list = list;
  }

  public int Count => _list.Count;

  public T this[int index] => _list[index];

  public override bool Equals(object obj)
  {
    return obj is RecordList<T> other && Equals(other);
  }

  public override int GetHashCode()
  {
    HashCode hashCode = new();
    foreach (T item in _list)
    {
      hashCode.Add(item);
    }

    return hashCode.ToHashCode();
  }

  public bool Equals(RecordList<T> other)
  {
    if (other.Count != Count)
      return false;

    for (int i = 0; i < Count; i++)
    {
      if (!_list[i].Equals(other._list[i]))
        return false;
    }

    return true;
  }

  public Enumerator GetEnumerator()
  {
    return new Enumerator(_list);
  }

  IEnumerator<T> IEnumerable<T>.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public static RecordList<T> Empty => new(Array.Empty<T>());

  public struct Enumerator : IEnumerator<T>
  {
    private readonly IReadOnlyList<T> _list;
    private int _index;

    public Enumerator(IReadOnlyList<T> list) : this()
    {
      _list = list;
      _index = -1;
    }

    public T Current => _list[_index];

    object IEnumerator.Current => Current;

    public void Dispose() { }

    public bool MoveNext()
    {
      return ++_index < _list.Count;
    }

    public void Reset()
    {
      _index = 0;
    }
  }
}
