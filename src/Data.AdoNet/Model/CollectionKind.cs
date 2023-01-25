using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Indicates the kind of a collection property.
/// </summary>
[Experimental]
public enum CollectionKind
{
  /// <summary>
  /// The property is not a collection.
  /// </summary>
  None,

  /// <summary>
  /// The property can be assigned from a <see cref="List{T}"/> instance.
  /// </summary>
  List,

  /// <summary>
  /// The property can be assigned from an <see cref="HashSet{T}"/> instance.
  /// </summary>
  HashSet,

  /// <summary>
  /// The property is a collection, but its type is unknown.
  /// </summary>
  Unknown
}
