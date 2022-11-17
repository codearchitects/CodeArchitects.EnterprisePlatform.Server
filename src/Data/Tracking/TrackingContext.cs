namespace CodeArchitects.Platform.Data.Tracking;

internal class TrackingContext : ITrackingContext
{
  private readonly Dictionary<object, TrackingState> _states;

  public TrackingContext()
  {
    _states = new();
  }

  public TrackingState GetTrackingState(object obj)
  {
    if (obj is null)
      throw new ArgumentNullException(nameof(obj));

    return _states.TryGetValue(obj, out TrackingState state)
      ? state
      : TrackingState.Detached;
  }

  public void SetTrackingState(object obj, TrackingState state)
  {
    if (obj is null)
      throw new ArgumentNullException(nameof(obj));

    _states[obj] = state;
  }
}
