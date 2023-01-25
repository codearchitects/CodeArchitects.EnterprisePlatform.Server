using AutoMapper;
using CodeArchitects.Platform.Data.Tracking;

namespace CodeArchitects.Platform.Data.AutoMapper;

internal class TrackingMappingAction<TSource, TDestination> : IMappingAction<TSource, TDestination>
  where TSource : ITrackable
  where TDestination : class
{
  private readonly ITrackingContext _trackingContext;

  public TrackingMappingAction(ITrackingContext trackingContext)
  {
    _trackingContext = trackingContext;
  }

  public void Process(TSource source, TDestination destination, ResolutionContext context)
  {
    if (source.TrackingState is TrackingState.Detached)
      return;

    _trackingContext.SetTrackingState(destination, source.TrackingState);
  }
}
