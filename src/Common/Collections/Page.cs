namespace CodeArchitects.Platform.Common.Collections;

public class Page<T>
{
  public int Count { get; }
  public bool HasNext { get; }
  public IEnumerable<T> Elements { get; }

  public Page(IEnumerable<T> elements, int count, bool hasNext)
  {
    Elements = elements ?? throw new ArgumentNullException(nameof(elements));
    Count = count;
    HasNext = hasNext;
  }

  public Page(IReadOnlyCollection<T> elements, bool hasNext)
  {
    Elements = elements ?? throw new ArgumentNullException(nameof(elements));
    HasNext = hasNext;
    Count = elements.Count;
  }

  public IEnumerator<T> GetEnumerator()
    => Elements.GetEnumerator();
}

public static class ReadOnlyCollectionExtensions
{
  public static Page<T> AsPage<T>(this IReadOnlyCollection<T> collection, bool hasNext)
    => new Page<T>(collection, hasNext);
}
