using System.Collections;

namespace CodeArchitects.Platform.Common.Collections;

/// <summary>
/// Represents a page of a collection.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public class Page<T> : IEnumerable<T>
{
  /// <summary>
  /// The number of elements in the collection.
  /// </summary>
  public int Count { get; }

  /// <summary>
  /// <c>true</c> if the original collection has more pages, <c>false</c> otherwise.
  /// </summary>
  public bool HasNext { get; }

  /// <summary>
  /// The elements of the page.
  /// </summary>
  public IEnumerable<T> Elements { get; }

  /// <summary>
  /// Creates a new <see cref="Page{T}"/> from an <see cref="IEnumerable{T}"/> and the number of elements it contains.
  /// </summary>
  /// <param name="elements">The elements of the page.</param>
  /// <param name="count">The number of elements in the collection.</param>
  /// <param name="hasNext"><c>true</c> if the original collection has more pages, <c>false</c> otherwise.</param>
  public Page(IEnumerable<T> elements, int count, bool hasNext)
  {
    Elements = elements ?? throw new ArgumentNullException(nameof(elements));
    Count = count;
    HasNext = hasNext;
  }

  /// <summary>
  /// Creates a new <see cref="Page{T}"/> from an <see cref="IReadOnlyCollection{T}"/>.
  /// </summary>
  /// <param name="collection">Contains the elements of the page and provides their count.</param>
  /// <param name="hasNext"><c>true</c> if the original collection has more pages, <c>false</c> otherwise.</param>
  public Page(IReadOnlyCollection<T> collection, bool hasNext)
  {
    Elements = collection ?? throw new ArgumentNullException(nameof(collection));
    HasNext = hasNext;
    Count = collection.Count;
  }

  /// <inheritdoc/>
  public IEnumerator<T> GetEnumerator()
  {
    return Elements.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)Elements).GetEnumerator();
  }
}

/// <summary>
/// Extension methods for creating instances of <see cref="Page{T}"/>.
/// </summary>
public static class ReadOnlyCollectionExtensions
{
  /// <summary>
  /// Creates a new <see cref="Page{T}"/> from an <see cref="IReadOnlyCollection{T}"/>.
  /// </summary>
  /// <typeparam name="T">The element type.</typeparam>
  /// <param name="collection">The elements of the page.</param>
  /// <param name="hasNext"><c>true</c> if the original collection has more pages, <c>false</c> otherwise.</param>
  /// <returns>The created <see cref="Page{T}"/>.</returns>
  public static Page<T> AsPage<T>(this IReadOnlyCollection<T> collection, bool hasNext)
    => new(collection, hasNext);
}
