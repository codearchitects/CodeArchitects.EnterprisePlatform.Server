namespace CodeArchitects.Platform.Data.Features.Associations;

/// <summary>
/// Enumeration representing the kinds of associations that can exist between entities in a domain model.
/// </summary>
public enum AssociationKind
{
  /// <summary>
  /// An association between two entities that belong to the same aggregate.
  /// </summary>
  IntraAggregate,

  /// <summary>
  /// An association between two entities that belong to different aggregates.
  /// </summary>
  InterAggregate
}