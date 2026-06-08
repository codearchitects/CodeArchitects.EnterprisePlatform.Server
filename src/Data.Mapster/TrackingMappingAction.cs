using CodeArchitects.Platform.Data.Tracking;

namespace CodeArchitects.Platform.Data.Mapster;

internal class TrackingMappingAction<TSource, TDestination>
  where TSource : ITrackable
  where TDestination : class
{
  private readonly ITrackingContext _trackingContext;

  public TrackingMappingAction(ITrackingContext trackingContext)
  {
    _trackingContext = trackingContext;
  }

  public void Process(TSource source, TDestination destination)
  {
    _trackingContext.SetTrackingState(destination, source.TrackingState);
  }
}
