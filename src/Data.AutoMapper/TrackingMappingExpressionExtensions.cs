using CodeArchitects.Platform.Data.AutoMapper;
using CodeArchitects.Platform.Data.Tracking;

namespace AutoMapper;

/// <summary>
/// Extension methods for <see cref="IMappingExpression{TSource, TDestination}"/>.
/// </summary>
public static class TrackingMappingExpressionExtensions
{
  /// <summary>
  /// Instructs AutoMapper to preserve the <see cref="TrackingState"/> of an object when mapped to another one.
  /// </summary>
  /// <typeparam name="TSource">The source type</typeparam>
  /// <typeparam name="TDestination">The destination type</typeparam>
  /// <param name="mapping">The mapping expression.</param>
  /// <returns>The same <see cref="IMappingExpression{TSource, TDestination}"/> for further configuration.</returns>
  public static IMappingExpression<TSource, TDestination> PreserveTracking<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
    where TSource : ITrackable
    where TDestination : class
  {
    return mapping.AfterMap<TrackingMappingAction<TSource, TDestination>>();
  }
}
