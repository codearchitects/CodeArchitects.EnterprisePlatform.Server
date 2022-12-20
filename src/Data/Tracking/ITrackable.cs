namespace CodeArchitects.Platform.Data.Tracking;

/// <summary>
/// Represents an entity that can be tracked by a data context.
/// </summary>
public interface ITrackable
{
  /// <summary>
  /// Gets the current tracking state of the entity.
  /// </summary>
  TrackingState TrackingState { get; }
}
