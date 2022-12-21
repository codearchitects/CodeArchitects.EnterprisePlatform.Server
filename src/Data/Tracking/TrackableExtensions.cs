namespace CodeArchitects.Platform.Data.Tracking;

/// <summary>
/// Extension methods to perform mapping of <see cref="ITrackable"/> instances while preserving tracking information.
/// </summary>
public static class TrackableExtensions
{
  /// <summary>
  /// Applies the specified mapping to a trackable object while preserving tracking information.
  /// </summary>
  /// <typeparam name="TTrackable">The trackable type.</typeparam>
  /// <typeparam name="TDestination">The destination type.</typeparam>
  /// <param name="trackable">The trackable to map.</param>
  /// <param name="context">The tracking context.</param>
  /// <param name="mappingFunc">The mapping function.</param>
  /// <returns>The mapped object.</returns>
  public static TDestination Map<TTrackable, TDestination>(this TTrackable trackable, ITrackingContext context, Func<TTrackable, TDestination> mappingFunc)
    where TTrackable : ITrackable
    where TDestination : class
  {
    return MapCore(
      trackable ?? throw new ArgumentNullException(nameof(trackable)),
      context ?? throw new ArgumentNullException(nameof(context)),
      mappingFunc ?? throw new ArgumentNullException(nameof(mappingFunc)));
  }

  /// <summary>
  /// Applies the specified mapping to a collection of trackable objects while preserving tracking information.
  /// </summary>
  /// <typeparam name="TTrackable">The trackable type.</typeparam>
  /// <typeparam name="TDestination">The destination type.</typeparam>
  /// <param name="trackable">The trackable to map.</param>
  /// <param name="context">The tracking context.</param>
  /// <param name="mappingFunc">The mapping function.</param>
  /// <returns>The mapped list.</returns>
  public static List<TDestination> MapList<TTrackable, TDestination>(this IEnumerable<TTrackable> trackables, ITrackingContext context, Func<TTrackable, TDestination> mappingFunc)
    where TTrackable : ITrackable
    where TDestination : class
  {
    return MapCore(
      trackables ?? throw new ArgumentNullException(nameof(trackables)),
      context ?? throw new ArgumentNullException(nameof(context)),
      mappingFunc ?? throw new ArgumentNullException(nameof(mappingFunc)))
      .ToList();
  }

  /// <summary>
  /// Applies the specified mapping to a collection of trackable objects while preserving tracking information.
  /// </summary>
  /// <typeparam name="TTrackable">The trackable type.</typeparam>
  /// <typeparam name="TDestination">The destination type.</typeparam>
  /// <param name="trackable">The trackable to map.</param>
  /// <param name="context">The tracking context.</param>
  /// <param name="mappingFunc">The mapping function.</param>
  /// <returns>The mapped array.</returns>
  public static TDestination[] MapArray<TTrackable, TDestination>(this IEnumerable<TTrackable> trackables, ITrackingContext context, Func<TTrackable, TDestination> mappingFunc)
    where TTrackable : ITrackable
    where TDestination : class
  {
    return MapCore(
      trackables ?? throw new ArgumentNullException(nameof(trackables)),
      context ?? throw new ArgumentNullException(nameof(context)),
      mappingFunc ?? throw new ArgumentNullException(nameof(mappingFunc)))
      .ToArray();
  }


  private static TDestination MapCore<TTrackable, TDestination>(this TTrackable trackable, ITrackingContext context, Func<TTrackable, TDestination> mappingFunc)
    where TTrackable : ITrackable
    where TDestination : class
  {
    TDestination destination = mappingFunc(trackable);
    context.SetTrackingState(destination, trackable.TrackingState);
    return destination;
  }

  private static IEnumerable<TDestination> MapCore<TTrackable, TDestination>(this IEnumerable<TTrackable> trackables, ITrackingContext context, Func<TTrackable, TDestination> mappingFunc)
    where TTrackable : ITrackable
    where TDestination : class
  {
    return trackables.Select(trackable => trackable.MapCore(context, mappingFunc));
  }
}
