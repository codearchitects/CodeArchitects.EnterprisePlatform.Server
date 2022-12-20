namespace CodeArchitects.Platform.Data.Tracking;

/// <summary>
/// Represents the state of an entity in an abstract data context.
/// </summary>
public enum TrackingState : byte
{
  /// <summary>
  /// The entity is not being tracked by the data context.
  /// </summary>
  Detached,

  /// <summary>
  /// The entity has not been modified since it was attached to the data context.
  /// </summary>
  Unchanged,

  /// <summary>
  /// The entity is new and has not yet been persisted to the data store.
  /// </summary>
  Added,

  /// <summary>
  /// The entity has been modified and the changes have not yet been persisted to the data store.
  /// </summary>
  Modified,

  /// <summary>
  /// The entity has been marked for removal and the change has not yet been persisted to the data store.
  /// </summary>
  Removed
}
