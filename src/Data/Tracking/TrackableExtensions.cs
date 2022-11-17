namespace CodeArchitects.Platform.Data.Tracking;

public static class TrackableExtensions
{
  public static TDestination Map<TTrackable, TDestination>(this TTrackable trackable, ITrackingContext context, Func<TTrackable, TDestination> mappingFunc)
    where TTrackable : ITrackable
    where TDestination : class
  {
    return MapCore(
      trackable ?? throw new ArgumentNullException(nameof(trackable)),
      context ?? throw new ArgumentNullException(nameof(context)),
      mappingFunc ?? throw new ArgumentNullException(nameof(mappingFunc)));
  }

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

  public static IEnumerable<TDestination> MapCore<TTrackable, TDestination>(this IEnumerable<TTrackable> trackables, ITrackingContext context, Func<TTrackable, TDestination> mappingFunc)
    where TTrackable : ITrackable
    where TDestination : class
  {
    return trackables.Select(trackable => trackable.MapCore(context, mappingFunc));
  }
}
