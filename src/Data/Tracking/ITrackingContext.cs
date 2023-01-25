namespace CodeArchitects.Platform.Data.Tracking;

/// <summary>
/// Represents a context that can track the state of entities.
/// </summary>
public interface ITrackingContext
{
  /// <summary>
  /// Gets the current tracking state of the specified object.
  /// </summary>
  /// <param name="obj">The object to get the tracking state for.</param>
  /// <returns>The current tracking state of the object.</returns>
  TrackingState GetTrackingState(object obj);

  /// <summary>
  /// Sets the tracking state of the specified object.
  /// </summary>
  /// <param name="obj">The object to set the tracking state for.</param>
  /// <param name="state">The tracking state to set for the object.</param>
  void SetTrackingState(object obj, TrackingState state);
}
