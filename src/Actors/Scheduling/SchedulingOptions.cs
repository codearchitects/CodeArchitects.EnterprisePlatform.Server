namespace CodeArchitects.Platform.Actors.Scheduling;

public record SchedulingOptions(
  TimeSpan Timer,
  TimeSpan Period)
{
  public static readonly SchedulingOptions Now = new();

  public SchedulingOptions()
    : this(TimeSpan.Zero, Timeout.InfiniteTimeSpan)
  {
  }

  public static SchedulingOptions In(TimeSpan timer) => new() { Timer = timer };

  public static SchedulingOptions Every(TimeSpan period) => new() { Period = period };
}
