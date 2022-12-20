using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents the kind of association between two entities.
/// </summary>
[Experimental]
public enum AssociationKind // TODO: IMPORTANTE!!! Nel codice è stato usato tutto al contrario!
{
  /// <summary>
  /// Represents an aggregation association, where the child entity can exist independently of the parent entity.
  /// </summary>
  Aggregation,

  /// <summary>
  /// Represents a composition association, where the child entity cannot exist independently of the parent entity.
  /// </summary>
  Composition
}
