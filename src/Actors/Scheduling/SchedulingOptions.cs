namespace CodeArchitects.Platform.Actors.Scheduling;

public record SchedulingOptions(
  ScheduleId ScheduleId,
  TimeSpan Timer,
  TimeSpan Period)
{
  public static readonly SchedulingOptions Now = new();

  public SchedulingOptions()
    : this(default, TimeSpan.Zero, Timeout.InfiniteTimeSpan)
  {
  }

  public static SchedulingOptions In(TimeSpan timer) => new() { ScheduleId = ScheduleId.New(), Timer = timer };

  public static SchedulingOptions Every(TimeSpan period) => new() { ScheduleId = ScheduleId.New(), Period = period };
}
