using CodeArchitects.Platform.Data.Mapster;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.Extensions.DependencyInjection;

namespace Mapster;

/// <summary>
/// Extension methods for <see cref="TypeAdapterSetter{TSource, TDestination}"/>.
/// </summary>
public static class TrackingMappingExpressionExtensions
{
  /// <summary>
  /// Instructs Mapster to preserve the <see cref="TrackingState"/> of an object when mapped to another one.
  /// </summary>
  /// <typeparam name="TSource">The source type.</typeparam>
  /// <typeparam name="TDestination">The destination type.</typeparam>
  /// <param name="mapping">The mapping expression.</param>
  /// <param name="trackingContext">The tracking context used to set the tracking state on the destination object.</param>
  /// <returns>The same <see cref="TypeAdapterSetter{TSource, TDestination}"/> for further configuration.</returns>
  public static TypeAdapterSetter<TSource, TDestination> PreserveTracking<TSource, TDestination>(this TypeAdapterSetter<TSource, TDestination> mapping, ITrackingContext trackingContext)
    where TSource : ITrackable
    where TDestination : class
  {
    var action = new TrackingMappingAction<TSource, TDestination>(trackingContext);
    return mapping.AfterMapping((src, dest) => action.Process(src, dest));
  }

  /// <summary>
  /// Instructs Mapster to preserve the <see cref="TrackingState"/> of an object when mapped to another one.
  /// </summary>
  /// <typeparam name="TSource">The source type.</typeparam>
  /// <typeparam name="TDestination">The destination type.</typeparam>
  /// <param name="mapping">The mapping expression.</param>
  /// <returns>The same <see cref="TypeAdapterSetter{TSource, TDestination}"/> for further configuration.</returns>
  /// <exception cref="InvalidOperationException">Thrown when no <see cref="ITrackingContext"/> is available in the current Mapster mapping context.</exception>
  public static TypeAdapterSetter<TSource, TDestination> PreserveTracking<TSource, TDestination>(this TypeAdapterSetter<TSource, TDestination> mapping)
    where TSource : ITrackable
    where TDestination : class
  {
    return mapping.AfterMapping(static (src, dest) =>
    {
      MapContext? mapContext = MapContext.Current;
      IServiceProvider? serviceProvider = mapContext?.Parameters.TryGetValue("Mapster.DependencyInjection.sp", out object? parameter) == true
        ? parameter as IServiceProvider
        : null;
      ITrackingContext trackingContext = serviceProvider?.GetService<ITrackingContext>()
        ?? throw new InvalidOperationException($"Unable to resolve {nameof(ITrackingContext)} from the current Mapster context. Use ServiceMapper or call the overload that accepts an explicit tracking context.");

      var action = new TrackingMappingAction<TSource, TDestination>(trackingContext);
      action.Process(src, dest);
    });
  }
}
