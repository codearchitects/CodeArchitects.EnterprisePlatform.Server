using CodeArchitects.Platform.Data.AutoMapper;
using CodeArchitects.Platform.Data.Tracking;

namespace AutoMapper;

public static class TrackingMappingExpressionExtensions
{
  public static IMappingExpression<TSource, TDestination> PreserveTracking<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
    where TSource : ITrackable
    where TDestination : class
  {
    return mapping.AfterMap<TrackingMappingAction<TSource, TDestination>>();
  }
}
