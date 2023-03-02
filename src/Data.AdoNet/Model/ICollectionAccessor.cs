using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a mechanism to access the elements in a collection.
/// </summary>
[Experimental]
public interface ICollectionAccessor
{
  /// <summary>
  /// Adds an element to the collection.
  /// </summary>
  /// <param name="instance">The instance of the collection.</param>
  /// <param name="value">The value to add to the collection.</param>
  void Add(object instance, object value);

  /// <summary>
  /// Removes an element from the collection.
  /// </summary>
  /// <param name="instance">The instance of the collection.</param>
  /// <param name="value">The value to remove from the collection.</param>
  void Remove(object instance, object value);

  /// <summary>
  /// Tries to get the number of elements in the collection, without enumerating them.
  /// </summary>
  /// <param name="instance">The instance of the collection.</param>
  /// <param name="count">The number of elements in the collection, if successful, 0 otherwise.</param>
  /// <returns><see langword="true"/> if the count can be obtained without enumerating the collection, <see langword="false"/> otherwise.</returns>
  bool TryGetNonEnumeratedCount(object instance, out int count);
}
