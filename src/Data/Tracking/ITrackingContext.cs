namespace CodeArchitects.Platform.Data.Tracking;

public interface ITrackingContext
{
  TrackingState GetTrackingState(object obj);
  void SetTrackingState(object obj, TrackingState state);
}
